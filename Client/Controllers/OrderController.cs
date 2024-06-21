using Client.Extentions;
using Microsoft.AspNetCore.Mvc;
using PFood.Data;
using PFood.Models;
using System.Net.Http.Headers;

namespace Client.Controllers
{
    public class OrderController : Controller
    {
        private HttpClient _httpClient;
        public OrderController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient.CreateClient("MyApi");

        }
        public async Task<IActionResult> Index(string? fullname, string? status)
        {
            SetAuthorizationHeader();
            var user = HttpContext.Session.GetObjectFromJson<LoginResponse>("Account");
            if (user == null) return RedirectToAction("Login", "Account");
            var response = await _httpClient.GetAsync($"Orders?fullname={fullname}&status={status}");
            if (response.IsSuccessStatusCode)
            {
                var listOrder = await response.Content.ReadFromJsonAsync<List<OrderVM>>();
                ViewBag.Status = status;
                ViewBag.Fullname = fullname;
                return View(listOrder.Where(x=>x.UserID == user.UserID).OrderByDescending(x => x.OrderDate).ToList());
            }
            return View(new List<OrderVM>());
        }

        public async Task<IActionResult> Detail(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"Orders/{id}");
            if (response.IsSuccessStatusCode)
            {               
                var order = await response.Content.ReadFromJsonAsync<Order>();
                if(order?.CouponID != null)
                {
                    var rpCoupon = await _httpClient.GetAsync($"Coupons/{order.CouponID}");
                    if (rpCoupon.IsSuccessStatusCode)
                    {
                        var coupon = await rpCoupon.Content.ReadFromJsonAsync<CouponVM>();
                        ViewBag.Coupon = coupon;    
                    }
                }
                return View(order);
            }
            return View(new Order());
        }
        public async Task<IActionResult> Cancel(int id, string status)
        {
            SetAuthorizationHeader();
            if (id <= 0 || string.IsNullOrEmpty(status))
            {
                TempData["ErrorMessage"] = "Invalid parameters.";
                return RedirectToAction("Index");
            }

            var response = await _httpClient.PutAsync($"Orders/update-status?id={id}&status={status}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cancel order successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Cancel order failed";
            }

            return RedirectToAction("Index");
        }
        private void SetAuthorizationHeader()
        {
            string token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
