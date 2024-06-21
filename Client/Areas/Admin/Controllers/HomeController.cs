using Client.Extentions;
using Microsoft.AspNetCore.Mvc;
using PFood.Data;
using PFood.Models;
using System.Net.Http;

namespace Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class HomeController : Controller
    {
		private readonly HttpClient _httpClient;
		public HomeController(IHttpClientFactory httpClientF)
		{
            _httpClient = httpClientF.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index()
        {
            var loginResponse = HttpContext.Session.GetObjectFromJson<LoginResponse>("Account");
            if (loginResponse != null)
            {
                if(loginResponse.Role != "Admin")
                {
                    return RedirectToAction("AccessDenied","Home");
                }
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer",loginResponse.Token);
                var response = await _httpClient.GetAsync($"auth/{loginResponse.UserID}");
                if(response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserVM>();
                    if (user != null)
                    {
                        HttpContext.Session.SetString("email",user.Email);
                        if(user.Avartar != null)
                        {
                            HttpContext.Session.SetString("avatar", user.Avartar);
                        }
                        
                    }
                }
                var responseOrder = await _httpClient.GetAsync($"Orders");
                if (responseOrder.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Welcome to Admin Dashboard.";
                    var listOrder = await responseOrder.Content.ReadFromJsonAsync<List<OrderVM>>();

                    var countOrderToday = listOrder.Where(x=>x.OrderDate.Date == DateTime.Today).Count();

                    var countOrder = listOrder.Count();

                    var amountToday = listOrder.Where(x => x.OrderDate.Date == DateTime.Today).Sum(x => x.TotalAmount);

                    var amount = listOrder.Sum(x => x.TotalAmount);

                    var listOrderChart = new List<KeyValuePair<string,int>>(); //list ngày và số đơn hàng

                    ViewBag.countOrderToday = countOrderToday;
                    ViewBag.countOrder = countOrder;
                    ViewBag.amountToday = amountToday;
                    ViewBag.amount = amount;

                    var currentDay = DateTime.Today;

                    for (int i = 6; i >= 0; i--)
                    {
                        var date = currentDay.AddDays(-i);
                        var orderC = listOrder.Where(x => x.OrderDate.Date == date).Count();
                        var chart = new KeyValuePair<string,int>(date.ToString("dd/MM"),orderC);
                        listOrderChart.Add(chart);
                    }

                    ViewBag.ChartData = listOrderChart;

                    var totalAmountChart = new List<KeyValuePair<string, decimal>>(); //list ngày và số tiền

                    for (int i = 6; i >= 0; i--)
                    {
                        var date = currentDay.AddDays(-i);
                        var total = listOrder.Where(x => x.OrderDate.Date == date).Sum(x => x.TotalAmount);
                        var chart = new KeyValuePair<string, decimal>(date.ToString("dd/MM"), total);
                        totalAmountChart.Add(chart);
                    }

                    ViewBag.ChartDataAmount = totalAmountChart;


                   
                    return View(listOrder.OrderByDescending(x => x.OrderDate).Take(5).ToList());
                }
                return View(new List<OrderVM>());      
            }
            else
            {
               
                TempData["ErrorMessage"] = "Please Login.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }
           
        }
        public async Task<IActionResult> Feedback()
        {
            string token = HttpContext.Session.GetString("JwtToken");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("Feedbacks");
            if (response.IsSuccessStatusCode)
            {
                var listFeedback = await response.Content.ReadFromJsonAsync<List<Feedback>>();
                if (listFeedback != null)
                {
                    ViewBag.Feedback = listFeedback;
                }
            }
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
