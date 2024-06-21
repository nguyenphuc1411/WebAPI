using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using PFood.Data;
using PFood.Models;

namespace Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientF)
        {
            _httpClient = httpClientF.CreateClient("MyApi");
        }

        public async  Task<IActionResult> Index()
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync("auth/getalluser");
            if(response.IsSuccessStatusCode)
            {
                var listUser = await response.Content.ReadFromJsonAsync<List<UserVM>>();
                return View(listUser);
            }
            return View(new List<UserVM>());
        }
        public async Task<IActionResult> Delete(string id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"auth/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Deleted successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "Deleted failed.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> EditRole(string id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"auth/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<UserVM>();
                if (user != null)
                {
                    return View(user);
                }
                TempData["ErrorMessage"] = "An error.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "An error.";
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditRole(string id,string role)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"auth/setrole/{id}",role);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Updated Role Successflly";
                return RedirectToAction("Index");
            }
            TempData["ErrorMessage"] = "An error.";
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
