using Microsoft.AspNetCore.Mvc;
using PFood.Data;
using PFood.Models;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly HttpClient _httpClient;
        public OrderController(IHttpClientFactory httpClientF)
        {
            _httpClient = httpClientF.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index(string? fullname, string? status)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"Orders?fullname={fullname}&status={status}");
            if (response.IsSuccessStatusCode)
            {
                var listOrder = await response.Content.ReadFromJsonAsync<List<OrderVM>>();
                ViewBag.Status = status;
                ViewBag.Fullname = fullname;
                return View(listOrder.OrderByDescending(x => x.OrderDate).ToList());
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
                if (order?.CouponID != null)
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
        public async Task<IActionResult> Confirm(int id)
        {

            SetAuthorizationHeader();     
            var response = await _httpClient.PutAsync($"Orders/update-status?id={id}&status=Confirmed", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Confirm order successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Confirm order failed";
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
