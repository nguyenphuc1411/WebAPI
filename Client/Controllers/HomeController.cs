using Client.Models;
using Microsoft.AspNetCore.Mvc;
using PFood.Data;
using PFood.Models;
using System.Diagnostics;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
		private readonly HttpClient _httpClient;
        public HomeController(ILogger<HomeController> logger,IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("MyApi");
        }

		public async Task<IActionResult> Index()
		{
			var response = await _httpClient.GetAsync("Foods/news");
			if (response.IsSuccessStatusCode)
			{
				var listNew = await response.Content.ReadFromJsonAsync<List<FoodVM>>();
				var responseBest = await _httpClient.GetAsync("Foods/bestselling");
				if (responseBest.IsSuccessStatusCode)
				{
					var bestSelling = await responseBest.Content.ReadFromJsonAsync<FoodVM>();
					ViewBag.BestSelling = bestSelling;
				}
				return View(listNew);
			}     
            return View(new List<FoodVM>());
		}


		public IActionResult Contact()
		{
			return View();
		}
		public IActionResult About()
		{
			return View();
		}
        [HttpPost]
        public async Task<IActionResult> Contact(Feedback feedback)
        {
            if(ModelState.IsValid)
            {
				var response = await _httpClient.PostAsJsonAsync("Feedbacks", feedback);
				if (response.IsSuccessStatusCode)
				{
					TempData["SuccessMessage"] = "Thanks for your feedback!";
					return RedirectToAction("Index");
				}
                else
                {
					TempData["SuccessMessage"] = "Send feedback failed";
					return View(feedback);
				}
			}
            return View(feedback);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
