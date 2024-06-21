using Client.Extentions;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using PagedList;
using PFood.Data;
using PFood.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;

namespace Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FoodController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment webHostEnvironment;
        public FoodController(IHttpClientFactory httpClientF, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientF.CreateClient("MyApi");
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(string search, int page = 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync("Foods");
            if (response.IsSuccessStatusCode)
            {
                var foods = await response.Content.ReadFromJsonAsync<List<FoodVM>>();

                if (!string.IsNullOrEmpty(search))
                {
                    foods = foods?.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                }
                var pagedList = foods.ToPagedList(page, pageSize);
                return View(new PagedResponse<FoodVM>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = pagedList.PageCount,
                    TotalProducts = pagedList.TotalItemCount,
                    Data = pagedList
                });
            }

            // Handle the error appropriately (e.g., log it, show an error message)
            return View(new List<FoodVM>());
        }      
        public async Task<IActionResult> Create()
        {
            var response = await _httpClient.GetAsync("Categories");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<CategoryVM>>();
                ViewBag.ListCategory = categories;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(FoodVM foodVM, IFormFile imageFile,IFormFile? imageFile1, IFormFile? imageFile2, IFormFile? imageFile3)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.IsValidImageFile())
                {
                    string newFileName = Path.GetFileName(imageFile.FileName);
                 
                    string imageFullPath = Path.Combine(webHostEnvironment.WebRootPath, "assets/img/foods", newFileName);
                    try
                    {
                        using (var stream = new FileStream(imageFullPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                            foodVM.ImageMain = newFileName;
                            if (imageFile1 != null && imageFile1.IsValidImageFile())
                            {
                                string newFileName1 = Path.GetFileName(imageFile1.FileName);
                                await imageFile1.CopyToAsync(stream);
                                foodVM.ImageFirst = newFileName1;
                            }
                            if (imageFile2 != null && imageFile2.IsValidImageFile())
                            {
                                string newFileName2 = Path.GetFileName(imageFile1.FileName);
                                await imageFile2.CopyToAsync(stream);
                                foodVM.ImageSecond = newFileName2;
                            }
                            if (imageFile3 != null && imageFile3.IsValidImageFile())
                            {
                                string newFileName3 = Path.GetFileName(imageFile3.FileName);
                                await imageFile3.CopyToAsync(stream);
                                foodVM.ImageThree = newFileName3;
                            }                          
                        }
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "An error occurred while creating.";
                        ModelState.AddModelError("", "Error saving the image file");
                        return View(foodVM);
                    }
                    SetAuthorizationHeader();
                    var res = await _httpClient.PostAsJsonAsync("Foods", foodVM);

                    if (res.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Created successfully.";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "An error occurred while creating.";
                    ModelState.AddModelError("errorImgae", "Invalid Image");
                }
               
            }
            var response = await _httpClient.GetAsync("Categories");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<CategoryVM>>();
                ViewBag.ListCategory = categories;
            }
            return View(foodVM);
        }

        public async Task<IActionResult> Edit(int id)
        {
            SetAuthorizationHeader();
            var res = await _httpClient.GetAsync($"Foods/{id}");

            if (res.IsSuccessStatusCode && res.Content != null)
            {
                var food = await res.Content.ReadFromJsonAsync<FoodVM>();
                var response = await _httpClient.GetAsync("Categories");
                if (response.IsSuccessStatusCode)
                {
                    var categories = await response.Content.ReadFromJsonAsync<List<CategoryVM>>();
                    ViewBag.ListCategory = categories;
                }
                return View(food);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, FoodVM foodVM, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(foodVM);
            }
            string newFileName, imageFullPath;
            if (imageFile != null && imageFile.IsValidImageFile())
            {
                newFileName = Path.GetFileName(imageFile.FileName);
                imageFullPath = Path.Combine(webHostEnvironment.WebRootPath, "assets/img/foods", newFileName);
                try
                {
                    using (var stream = new FileStream(imageFullPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                        foodVM.ImageMain = newFileName;
                    }
                }
                catch
                {
                    TempData["ErrorMessage"] = "An error occurred while updating.";
                    ModelState.AddModelError("", "Error saving the image file");
                    return View(foodVM);
                }
            }
            else
            {
                if (imageFile != null)
                {
                    ModelState.AddModelError("", "Invalid Image");
                };
            }
            SetAuthorizationHeader();
            var res = await _httpClient.PutAsJsonAsync($"Foods/{id}", foodVM);

            if (res.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Updated successfully.";
                return RedirectToAction("Index");
            }
            ModelState.AddModelError(string.Empty, "Do not have any change");
            return View(foodVM);
        }
        public async Task<IActionResult> Delete(int id)
        {
            if(IsAdmin())
            {
                SetAuthorizationHeader();
                var response = await _httpClient.DeleteAsync($"Foods/{id}");

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
            return RedirectToAction("Login", "Account", new { area = "" });
        }
        private void SetAuthorizationHeader()
        {
            string token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
        private bool IsAdmin()
        {
            var loginResponse = HttpContext.Session.GetObjectFromJson<LoginResponse>("Account");
            if (loginResponse.Role == "Admin")
            {
                return true;
            }
            else { return false; }
        }
    }
}
