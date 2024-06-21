using Client.Extentions;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using PagedList;
using PFood.Data;
using PFood.Models;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Client.Controllers
{
    public class FoodController : Controller
    {
        private readonly HttpClient _httpClient;

        public FoodController(IHttpClientFactory httpClient, SignInManager<User> signInManager)
        {
            _httpClient = httpClient.CreateClient("MyApi");
        }

        public async Task<IActionResult> Index(string search, decimal? priceMin, decimal? priceMax, int? categoryID, string sortOrder, int page = 1, int pageSize = 8)
        {
            var resCate = await _httpClient.GetAsync("Categories");
            if (resCate.IsSuccessStatusCode)
            {
                var listCategories = await resCate.Content.ReadFromJsonAsync<List<CategoryVM>>();
                ViewBag.Categories = listCategories.ToList();
            }
            else
            {
                ViewBag.Categories = new List<CategoryVM>();
            }
            var response = await _httpClient.GetAsync("Foods");
            if (response.IsSuccessStatusCode)
            {
                var foods = await response.Content.ReadFromJsonAsync<List<FoodVM>>();

                /*Filter Categories*/
                if (categoryID.HasValue)
                {
                    foods = foods.Where(x => x.CategoryID == categoryID).ToList();
                    ViewBag.CategoryID = categoryID;
                }

                if (!string.IsNullOrEmpty(search))
                {
                    foods = foods?.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                }

                if (priceMin.HasValue || priceMax.HasValue)
                {
                    if (priceMin.HasValue)
                    {
                        if (priceMax.HasValue)
                        {
                            foods = foods.Where(x => x.Price >= priceMin && x.Price <= priceMax).ToList();
                        }
                        foods = foods.Where(x => x.Price >= priceMin).ToList();
                    }
                    else
                    {
                        foods = foods.Where(x => x.Price <= priceMax).ToList();
                    }
                    ViewBag.priceMin = priceMin;
                    ViewBag.priceMax = priceMax;
                }
                if (!string.IsNullOrEmpty(sortOrder))
                {
                    ViewBag.sortOrder = sortOrder;
                    switch (sortOrder)
                    {
                        case "priceAsc":
                            {
                                foods = foods.OrderBy(x => x.Price).ToList();
                                break;
                            }
                        case "priceDesc":
                            {
                                foods = foods.OrderByDescending(x => x.Price).ToList();
                                break;
                            }
                        case "nameAsc":
                            {
                                foods = foods.OrderBy(x => x.Name).ToList();
                                break;
                            }
                        case "nameDesc":
                            {
                                foods = foods.OrderByDescending(x => x.Name).ToList();
                                break;
                            }
                        default:
                            break;
                    }
                }
                var pagedList = foods.Where(x => x.Status == true).ToPagedList(page, pageSize);
                return View(new PagedResponse<FoodVM>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = pagedList.PageCount,
                    TotalProducts = pagedList.TotalItemCount,
                    Data = pagedList
                });
            }

            return View(new PagedResponse<FoodVM>());
        }
        public async Task<IActionResult> Detail(int id)
        {
            var response = await _httpClient.GetAsync($"Foods/{id}");
            if ((response.IsSuccessStatusCode))
            {
                // find food
                var food = await response.Content.ReadFromJsonAsync<Food>();
                var responseCate = await _httpClient.GetAsync($"Categories/{food.CategoryID}");
                if ((responseCate.IsSuccessStatusCode))
                {
                    var cateVM = await responseCate.Content.ReadFromJsonAsync<CategoryVM>();
                    ViewBag.NameCategory = cateVM.Name;
                }
                var responseReview = await _httpClient.GetAsync($"Reviews/{food.FoodID}");
                if ((responseReview.IsSuccessStatusCode))
                {
                    var reviews = await responseReview.Content.ReadFromJsonAsync<List<ReviewVM>>();
                    reviews = reviews.OrderByDescending(x => x.CreatedDate).ToList();
                    ViewBag.ListReview = reviews;
                }
                return View(food);
            }



            return NotFound();
        }
        public async Task<IActionResult> Rating(int rating, string comment, int foodID)
        {
            string token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Please login to rating";
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrEmpty(comment))
            {
                TempData["ErrorMessage"] = "Please input comment";
                return RedirectToAction("Detail", new { id = foodID });
            }

            var user = HttpContext.Session.GetObjectFromJson<LoginResponse>("Account");
            if (user != null)
            {
                var reviewVM = new ReviewVM
                {
                    Rating = rating,
                    Comment = comment,
                    FoodID = foodID,
                    UserID = user.UserID
                };
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("Reviews", reviewVM);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Rating successflly";
                }
                else
                {
                    TempData["ErrorMessage"] = "Rating failed";
                }
            }
            return RedirectToAction("Detail", new { id = foodID });
        }
    }
}
