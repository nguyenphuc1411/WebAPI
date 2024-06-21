using Client.Extentions;
using Client.Models;
using Client.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Common;
using PFood.Data;
using PFood.Models;
using System.Net.Http.Headers;
using System.Text;

namespace Client.Controllers
{
    public class CartsController : Controller
    {
        private readonly HttpClient _httpClient;

        private readonly IVnPayService vnPayService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartsController(IHttpClientFactory httpClientF, IVnPayService vnPayService, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClientF.CreateClient("MyApi");
            this.vnPayService = vnPayService;        
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            List<CartVM> carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");

            if (carts != null)
            {
                ViewBag.SubTotal = carts.Sum(x => x.Quantity * x.Price);
                return View(carts);
            }
            return View(new List<CartVM>());
        }
        public IActionResult AddToCart(int FoodID, string Name, string Image, decimal Price, int quantity = 1)
        {
            List<CartVM> carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");

            if (carts == null)
            {
                carts = new List<CartVM>();
            }

            // Search food in cart
            int index = carts.FindIndex(c => c.FoodID == FoodID);
            if (index != -1)
            {
                carts[index].Quantity += quantity;
                TempData["SuccessMessage"] = "Update cart successfully";
            }
            else
            {
                carts.Add(new CartVM
                {
                    FoodID = FoodID,
                    Name = Name,
                    Image = Image,
                    Price = Price,
                    Quantity = quantity
                });
                TempData["SuccessMessage"] = "Added to cart successfully";
            }

            HttpContext.Session.SetObjectAsJson("cart", carts);

            HttpContext.Session.SetObjectAsJson("CartCount", carts.Count);

            return RedirectToAction("Index", "Food");
        }
        public IActionResult Remove(int id)
        {
            List<CartVM> carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
            if (carts.Count == 1)
            {
                HttpContext.Session.SetObjectAsJson("cart", "");
                TempData["SuccessMessage"] = "Deleted successfully.";
                HttpContext.Session.SetObjectAsJson("CartCount", 0);
            }
            else
            {
                int index = carts.FindIndex(x => x.FoodID == id);
                if (index != -1)
                {
                    carts.RemoveAt(index);
                    HttpContext.Session.SetObjectAsJson("cart", carts);
                    TempData["SuccessMessage"] = "Deleted successfully.";
                    HttpContext.Session.SetObjectAsJson("CartCount", carts.Count);
                }
                else
                {
                    TempData["ErrorMessage"] = "An error while deleting.";
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ApplyCoupon(string couponCode)
        {
            var response = await _httpClient.GetAsync($"Coupons/code/{couponCode}");
            if (response.IsSuccessStatusCode)
            {
                var coupon = await response.Content.ReadFromJsonAsync<CouponVM>();
                if (coupon != null)
                {
                    HttpContext.Session.SetObjectAsJson("coupon", coupon);
                    TempData["Coupon"] = coupon.Discount.ToString();
                    TempData["SuccessMessage"] = "Apply coupon successfully";
                }
                else
                {
                    TempData["ErrorMessage"] = "This coupon is not available";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "This coupon is not available";
            }
            return RedirectToAction("Index");
        }
        public IActionResult Increase(int id)
        {
            List<CartVM> carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
            int index = carts.FindIndex(x => x.FoodID == id);
            if (index != -1)
            {
                carts[index].Quantity++;
                HttpContext.Session.SetObjectAsJson("cart", carts);
                TempData["SuccessMessage"] = "Increase quantity successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "An error while updating";
            }

            return RedirectToAction("Index");
        }
        public IActionResult Decrease(int id)
        {
            List<CartVM> carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
            int index = carts.FindIndex(x => x.FoodID == id);
            if (index != -1)
            {
                if (carts[index].Quantity > 1)
                {
                    carts[index].Quantity--;
                    HttpContext.Session.SetObjectAsJson("cart", carts);

                }
                else
                {
                    TempData["ErrorMessage"] = "Can't update because quantity is minimum";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "An error while updating";
            }

            return RedirectToAction("Index");
        }

        public IActionResult CheckOut()
        {
            string token = HttpContext.Session.GetString("JwtToken");
            if (token == null)
            {
                TempData["ErrorMessage"] = "Please login before checkout";
                return RedirectToAction("Login", "Account");
            }


            List<CartVM> carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
            if (carts == null)
            {
                TempData["ErrorMessage"] = "There are no product in the cart";
                return RedirectToAction("Index");
            }
            ViewBag.SubTotal = carts.Sum(x => x.Price * x.Quantity);
            ViewBag.Carts = carts;
            CouponVM coupon = HttpContext.Session.GetObjectFromJson<CouponVM>("coupon");
            if (coupon != null)
            {
                ViewBag.Coupon = coupon;
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CheckOut([Bind("FullName,PhoneNumber,DeliveryAddress,TotalAmount,CouponID,Note")] OrderVM orderVM, string PaymentMethod = "COD")
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra người dùng đăng nhập
                var user = HttpContext.Session.GetObjectFromJson<LoginResponse>("Account");
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Please Login";
                    return RedirectToAction("Login", "Account");
                }
                if(PaymentMethod == "VnPay")
                {

                    var orderPayment = orderVM;
                    orderPayment.UserID = user.UserID;
                    orderPayment.Status = "Paid";
                    // Lấy giỏ hàng từ session
                    var cartPs = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
                    if (cartPs != null)
                    {
                        foreach (var item in cartPs)
                        {
                            var orderDetailVM = new OrderDetailVM
                            {
                                FoodID = item.FoodID,
                                Price = item.Price,
                                Quantity = item.Quantity
                            };
                            orderPayment.OrderDetailVMs.Add(orderDetailVM);
                        }
                    }
                    HttpContext.Session.SetObjectAsJson("orderPayment", orderPayment);
                    var model = new VnPaymentRequestModel
                    {
                        Fullname = orderVM.FullName,
                        Amount = orderVM.TotalAmount,
                        CreatedDate = DateTime.Now,
                        Description = $"{orderVM.FullName} {orderVM.PhoneNumber}",
                        OrderID = new Random().Next(1000, 10000)
                    };
                    return Redirect(vnPayService.CreatePaymentUrl(_httpContextAccessor.HttpContext, model));
                }

                // Thiết lập Header Authorization
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);

                // Gán các giá trị cần thiết cho đơn hàng
                orderVM.UserID = user.UserID;
                orderVM.Status = "Wait for confirmation";

                // Lấy giỏ hàng từ session
                var carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
                if (carts != null)
                {
                    foreach (var item in carts)
                    {
                        var orderDetailVM = new OrderDetailVM
                        {
                            FoodID = item.FoodID,
                            Price = item.Price,
                            Quantity = item.Quantity
                        };
                        orderVM.OrderDetailVMs.Add(orderDetailVM);
                    }
                }

                // Gửi dữ liệu đơn hàng tới API
                var response = await _httpClient.PostAsJsonAsync("Orders", orderVM);
                if (response.IsSuccessStatusCode)
                {
                    var id = orderVM.OrderID;
                    TempData["SuccessMessage"] = "Place Order Successfully";

                    // Xóa session giỏ hàng
                    HttpContext.Session.Remove("cart");
                    HttpContext.Session.SetString("CartCount", "0"); // Cập nhật CartCount

                    return RedirectToAction("Index", "Order");
                }

                TempData["ErrorMessage"] = "CheckOut failed";
                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ, trả về lại view với thông tin giỏ hàng
            var cartsFromSession = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
            ViewBag.SubTotal = cartsFromSession?.Sum(x => x.Price * x.Quantity) ?? 0;
            ViewBag.carts = cartsFromSession;

            var coupon = HttpContext.Session.GetObjectFromJson<CouponVM>("coupon");
            ViewBag.Coupon = coupon;

            return View(orderVM);
        }


        public async Task<IActionResult> PaymentReturn()
        {
            var response = vnPayService.PaymentExecute(Request.Query);

            if(response == null || response.VnPayResponseCode != "00")
            {
                TempData["ErrorMessge"] = "An error pay VnPay" + response.VnPayResponseCode;
                return RedirectToAction("CheckOut");
            }

            var user = HttpContext.Session.GetObjectFromJson<LoginResponse>("Account");

            var orderPayment = HttpContext.Session.GetObjectFromJson<OrderVM>("orderPayment");

            if(orderPayment!= null)
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Token);
                var responsePay = await _httpClient.PostAsJsonAsync("Orders", orderPayment);
                if (responsePay.IsSuccessStatusCode)
                {
                    // lay email user

                    var responseUser = await _httpClient.GetAsync($"auth/{user.UserID}");
                    if (responseUser.IsSuccessStatusCode)
                    {
                        var userRep = await responseUser.Content.ReadFromJsonAsync<UserVM>();

                        if (userRep != null)
                        {
                            // gửi mail chi tiet don hang
                            var emailVM = new EmailVM
                            {
                                ToEmail = userRep.Email,
                                Subject = "Place Order Successfully",
                                Body = await GenerateEmailBody(orderPayment)
                            };
                            var responseSendMail = await _httpClient.PostAsJsonAsync($"Email", emailVM);                          
                        }
                    }
                    TempData["SuccessMessage"] = "Place Order Successfully";

                    HttpContext.Session.Remove("cart");
                    HttpContext.Session.SetString("CartCount", "0");

                    return RedirectToAction("Index", "Order");
                }
                TempData["ErrorMessage"] = "CheckOut failed";
                return RedirectToAction("CheckOut", "Carts");
            }

            TempData["ErrorMessage"] = "CheckOut failed";
            return RedirectToAction("CheckOut","Carts");
        }


        private async Task<string> GenerateEmailBody(OrderVM orderVM)
        {
            var carts = HttpContext.Session.GetObjectFromJson<List<CartVM>>("cart");
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("table { width: 100%; border-collapse: collapse; }");
            sb.AppendLine("th, td { border: 1px solid black; padding: 8px; text-align: left; }");
            sb.AppendLine("th { background-color: #f2f2f2; }");
            sb.AppendLine("h1, h2 { color: #333; }");
            sb.AppendLine("p { color: #555; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Invoice Details</h1>");
            sb.AppendLine($"<p><strong>Customer Name:</strong> {orderVM.FullName}</p>");
            sb.AppendLine($"<p><strong>PhoneNumber:</strong> {orderVM.PhoneNumber}</p>");
            if(orderVM.Note!= null)
            {
                sb.AppendLine($"<p><strong>Note:</strong> {orderVM.Note}</p>");
            }
            sb.AppendLine($"<p><strong>Billing Address:</strong> {orderVM.DeliveryAddress}</p>");
            sb.AppendLine($"<p><strong>Date:</strong> {orderVM.OrderDate:dd-MM-yyyy}</p>");
            sb.AppendLine("<h2>Items</h2>");
            sb.AppendLine("<table>");
            sb.AppendLine("<tr> " +
                                "<th>Food Name</th>" +
                                   "<th>Quantity</th>" +
                                   "<th>Unit Price</th>" +
                                   "<th>Total Price</th>" +
                           "</tr>");

            if(carts!= null)
            {
                foreach (var item in carts)
                {
                    sb.AppendLine(
                        $"<tr>" +
                            $"<td>{item.Name} </td>" +
                            $"<td>{item.Quantity}</td>" +
                             $"<td>{item.Price}</td>" +
                            $"<td>{item.Quantity * item.Price} $</td>" +
                        $"</tr>");
                }
            }
           
          
            sb.AppendLine("</table>");

            if (orderVM.CouponID != null)
            {
                var response = await _httpClient.GetAsync($"Coupons/{orderVM.CouponID}");
                if (response.IsSuccessStatusCode)
                {
                    var coupon = await response.Content.ReadFromJsonAsync<CouponVM>();
                    sb.AppendLine($"<h4>Coupon: {coupon?.CouponCode}</h4>");
                    sb.AppendLine($"<h4>Discount: {coupon?.Discount} $</h4>");
                }
            }

            sb.AppendLine($"<h2>Total Amount: {orderVM.TotalAmount} $</h2>");

            sb.AppendLine($"<hr/>");
            sb.AppendLine($"<h4>Thank you for your order!</h4>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

    }

}
