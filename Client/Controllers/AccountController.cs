using Client.Extentions;
using Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PFood.Data;
using PFood.Models;
using System.Drawing;
using System.Net.Http.Headers;

namespace Client.Controllers
{
	public class AccountController : Controller
	{
		private readonly HttpClient _httpClient;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AccountController(IHttpClientFactory httpClientF, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientF.CreateClient("MyApi");
            this.webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Login()
		{
			return View();
		}
		[HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
			if (ModelState.IsValid)
			{
				var response = await _httpClient.PostAsJsonAsync("auth/login", login);
				if (response.IsSuccessStatusCode)
				{
					var responseLogin = await response.Content.ReadFromJsonAsync<LoginResponse>();
					HttpContext.Session.SetString("JwtToken", responseLogin.Token);
                    HttpContext.Session.SetObjectAsJson("Account", responseLogin);
                    TempData["SuccessMessage"] = "Login successfully";
					return RedirectToAction("Index", "Home");
				}
				else
				{
                    TempData["ErrorMessage"] = "Login failed";
					return View(login);
                }
			}
            return View(login);
        }
        public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
        public async Task<IActionResult> Register(RegisterVM model) 
        {
			if(ModelState.IsValid)
			{
				var response = await _httpClient.PostAsJsonAsync("auth/register", model);
				if (response.IsSuccessStatusCode)
				{
					TempData["SuccessMessage"] = "Register User Successfully";
					return RedirectToAction("Login");
				}
				else
				{
                    TempData["ErrorMessage"] = "Register User Failed";
                    return View(model);
                }
			}
            return View(model);
        }
        public async Task<IActionResult> Profile()
		{
			var id = HttpContext.Session.GetObjectFromJson<LoginResponse>("Account").UserID;
            SetAuthorizationHeader();

			var response = await _httpClient.GetAsync($"auth/{id}");
			if (response.IsSuccessStatusCode)
			{
				var user = await response.Content.ReadFromJsonAsync<UserVM>();
				if (user != null)
				{
					return View(user);
				}				
			}

			return View(new UserVM());
		}
		public IActionResult Logout()
		{
            HttpContext.Session.Remove("JwtToken");
            HttpContext.Session.Remove("Account");
            HttpContext.Session.Remove("cart");
            HttpContext.Session.Remove("CartCount");
            TempData["SuccessMessage"] = "Logout Successfully";
            return RedirectToAction("Login");
        }

        private void SetAuthorizationHeader()
        {
            string token = HttpContext.Session.GetString("JwtToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }
		public async Task<IActionResult> Edit(string id)
		{
			SetAuthorizationHeader();
            var response = await _httpClient.GetAsync($"auth/{id}");
            if (response.IsSuccessStatusCode)
            {
                var user = await response.Content.ReadFromJsonAsync<User>();
				var userVM = new UserVM
				{
					Fullname = user.Fullname,
					Email = user.Email,
					Address = user.Address,
					DOB = user.DOB,
					Avartar = user.Avartar,
					PhoneNumber = user.PhoneNumber
				};
				return View(userVM);
            }
			else
			{
				return View(new UserVM());
			}
        }
		[HttpPost]
        public async Task<IActionResult> Edit(string id,UserVM user, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    if (imageFile.IsValidImageFile())
                    {
                        string newFileName = Path.GetFileName(imageFile.FileName);
                        string imageFullPath = Path.Combine(webHostEnvironment.WebRootPath, "assets/img/profile", newFileName);
                        try
                        {
                            using (var stream = new FileStream(imageFullPath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(stream);
                                user.Avartar = newFileName;
                            }
                        }
                        catch
                        {
                            TempData["ErrorMessage"] = "An error occurred while creating.";
                            ModelState.AddModelError("", "Error saving the image file");
                            return View(user);
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while updatting.";
                        ModelState.AddModelError("", "Image Invalid");
                        return View(user);
                    }
                    
                }             
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"auth/updateuser/{id}", user);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Update Successflly";
                    return RedirectToAction("Profile", "Account", id);
                }
                TempData["ErrorMessage"] = "An error occurred while updating.";
                return View(user);

            }
            else
            {
                return View(user);
            }
        }
        public IActionResult RequestForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RequestForgotPassword(string email)
        {
            if(email != null)
            {
                var response = await _httpClient.PostAsJsonAsync("email/requestforgotpassword",email);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Password reset email sent to your email.";
                    string code = await response.Content.ReadFromJsonAsync<string>();
                    HttpContext.Session.SetString("code", code);
                    HttpContext.Session.SetString("email", email);
                    return RedirectToAction("ResetPassword");
                }
                else
                {
                    TempData["ErrorMessage"] = "An error";
                }
            }           
            return View();
        }
        public IActionResult ResetPassword(string email)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPassword reset, string code)
        {
            if (ModelState.IsValid)
            {
                string codeSave = HttpContext.Session.GetString("code");
                if(code != codeSave)
                {
                    TempData["ErrorMessage"] = "Confirmation Code wrong!";
                    return View(reset);
                }
                else
                {
                    var emailSave = HttpContext.Session.GetString("email");
                    var response = await _httpClient.PostAsync($"auth/resetpassword?email={emailSave}&password={reset.Password}", null);
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMessage"] = "Change password successfully!";
                        return RedirectToAction("Login","Account");
                    }
                }
             
            }
            return View(reset);
        }
    }
}
