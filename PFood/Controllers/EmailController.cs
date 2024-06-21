
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFood.Models;
using PFood.Services;

namespace PFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailRepository _repos;
        private readonly IAccountRepository _accountRepos;
        public EmailController(IEmailRepository repos, IAccountRepository accountRepos)
        {
            _repos = repos;
            _accountRepos = accountRepos;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail([FromBody] EmailVM model)
        {
            bool result = await _repos.SendEmailAsync(model);

            if (result)
            {
                return Ok(new { Success = true, Message = "Email sent successfully." });
            }
            else
            {
                return StatusCode(500, new { Success = false, Message = "Failed to send email." });
            }
        }
        [HttpPost("requestforgotpassword")]
        public async Task<IActionResult> SendResetPassword([FromBody] string email)
        {
            bool isExistEmail = await _accountRepos.IsExistsEmail(email);
            if (isExistEmail)
            {
                string code = GenerateRandomString(6);
                // Gửi email đặt lại mật khẩu
                var result = await _repos.SendEmaiResetPassword(email,code);
                if (result)
                {
                    return Ok(code);
                }
                else
                {
                    return BadRequest();
                }
            }
            return NotFound();

        }


        [HttpPost("resetpassword")]
        public async Task<ActionResult> ResetPassword(string email,string newpassword)
        {
            bool result = await _accountRepos.ResetPassword(email, newpassword);

            return result ? Ok() : BadRequest();
        }

        static string GenerateRandomString(int length)
        {
            const string chars = "0123456789";
            Random random = new Random();
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
