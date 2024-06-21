using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using PFood.Models;
using System.Text;
using PFood.Data;
using Client.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Client.Extentions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Http;
using PagedList;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http.Headers;

namespace Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment webHostEnvironment;

        public CategoryController(IHttpClientFactory httpClientF, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientF.CreateClient("MyApi");
            this.webHostEnvironment = webHostEnvironment;       
        }

        public async Task<IActionResult> Index(string search, int page= 1, int pageSize = 10)
        {
            var response = await _httpClient.GetAsync("Categories");
            if (response.IsSuccessStatusCode)
            {
                var categories = await response.Content.ReadFromJsonAsync<List<CategoryVM>>();

                if (!string.IsNullOrEmpty(search))
                {
                    categories = categories?.Where(x => x.Name.ToLower().Contains(search.ToLower())).ToList();
                }
                var pagedList = categories.ToPagedList(page, pageSize);
                return View( new PagedResponse<CategoryVM>
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalPages = pagedList.PageCount,
                    TotalProducts = pagedList.TotalItemCount,
                    Data = pagedList
                });
            }
            return View(new PagedResponse<CategoryVM>());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(CategoryVM category, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.IsValidImageFile())
                {
                    string newFileName = Path.GetFileName(imageFile.FileName);
                    string imageFullPath = Path.Combine(webHostEnvironment.WebRootPath, "assets/img/categories", newFileName);
                    try
                    {
                        using (var stream = new FileStream(imageFullPath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                            category.Image = newFileName;
                        }
                    }
                    catch
                    {
                        TempData["ErrorMessage"] = "An error occurred while creating.";
                        ModelState.AddModelError("", "Error saving the image file");
                        return View(category);
                    }
                    SetAuthorizationHeader();
                    var res = await _httpClient.PostAsJsonAsync("Categories", category);

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
            return View(category);
        }
        public async Task<IActionResult> Edit(int id)
        {
            var res = await _httpClient.GetAsync("Categories/" + id.ToString());

            if (res.IsSuccessStatusCode && res.Content != null)
            {
                var category = await res.Content.ReadFromJsonAsync<CategoryVM>();
                return View(category);
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryVM category, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }
            string newFileName, imageFullPath;
            if (imageFile != null && imageFile.IsValidImageFile())
            {
                newFileName = Path.GetFileName(imageFile.FileName);
                imageFullPath = Path.Combine(webHostEnvironment.WebRootPath, "assets/img/categories", newFileName);
                try
                {
                    using (var stream = new FileStream(imageFullPath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                        category.Image = newFileName;
                    }
                }
                catch
                {
                    TempData["ErrorMessage"] = "An error occurred while editing.";
                    ModelState.AddModelError("", "Error saving the image file");
                    return View(category);
                }
            }
            else
            {
                if (imageFile != null)
                {
                    TempData["ErrorMessage"] = "An error occurred while editing.";
                    ModelState.AddModelError("", "Invalid Image");
                };
            }
            SetAuthorizationHeader();
            var res = await _httpClient.PutAsJsonAsync($"Categories/{id}", category);

            if (res.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Updated successfully.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while editing.";
                ModelState.AddModelError(string.Empty, "Do not have any change");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"Categories/{id}");

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
