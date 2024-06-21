using Client.Models;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using PagedList;
using PFood.Models;
using System.Net.Http.Headers;

namespace Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CouponController : Controller
    {
        private readonly HttpClient _httpClient;
        public CouponController(IHttpClientFactory httpClientF)
        {
            _httpClient = httpClientF.CreateClient("MyApi");         
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            string token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("Coupons");
            if (response.IsSuccessStatusCode)
            {
                var coupons = await response.Content.ReadFromJsonAsync<List<CouponVM>>();

                if (!string.IsNullOrEmpty(search))
                {
                    coupons = coupons?.Where(x => x.CouponCode.ToLower().Contains(search.ToLower())).ToList();
                }
                var pagedList = coupons.ToPagedList(page, pageSize);
                return View(new PagedResponse<CouponVM>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = pagedList.PageCount,
                    TotalProducts = pagedList.TotalItemCount,
                    Data = pagedList
                });
            }

            return View(new PagedResponse<CouponVM>());
        }
        public IActionResult Create()
        {          
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CouponVM couponVM)
        {
            if (ModelState.IsValid)
            {
                string token = HttpContext.Session.GetString("JwtToken");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var res = await _httpClient.PostAsJsonAsync("Coupons", couponVM);

                if (res.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Created successfully.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Created failed.";
                }
            }
            return View(couponVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var res = await _httpClient.GetAsync($"Coupons/{id}");

            if (res.IsSuccessStatusCode && res.Content != null)
            {
                var coupon = await res.Content.ReadFromJsonAsync<CouponVM>();             
                return View(coupon);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CouponVM couponVM)
        {
            if (!ModelState.IsValid)
            {
                return View(couponVM);
            }
            string token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var res = await _httpClient.PutAsJsonAsync($"Coupons/{id}", couponVM);

            if (res.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Updated successfully.";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Do not have any change");
            return View(couponVM);
        }
        public async Task<IActionResult> Delete(int id)
        {
            string token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"Coupons/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while deleting.";
                return View("Error");
            }
        }
    }
}
