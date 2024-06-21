using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PFood.Context;
using PFood.Data;
using PFood.Models;
using PFood.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAccountRepository _repos;
        private readonly IConfiguration _configuration;

        public AuthController(IAccountRepository repos, IConfiguration config)
        {
            _repos = repos;
            _configuration = config;
        }


        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginVM loginVM)
        {
            var resultLogin = await _repos.Login(loginVM);
            if (resultLogin.Success)
            {
                string token = GenerateToken(loginVM,resultLogin.Role);
                var response = new LoginResponse
                {
                    Success= true,
                    UserID = resultLogin.UserID,
                    Token = token,
                    Role = resultLogin.Role,
                };

                return Ok(response);
            }
            return BadRequest("Login failed");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            bool result = await _repos.Register(model);
            if (result)
            {
               return Ok(new { message = "User registered successfully!" });
            }
            return BadRequest("Failed");
        }
        [HttpGet("getalluser")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<UserVM>>> GetUsers()
        {
            return Ok( await _repos.GetUsers() );
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(string id)
        {
            bool result = await _repos.Delete(id);
            return result ? NoContent() : BadRequest(); 
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserVM>> GetUser(string id)
        {
            var user = await _repos.GetById(id);
            if (user == null) return BadRequest();
            return Ok(user);
        }

        [HttpPut("setrole/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetRole(string id,[FromBody] string role)
        {
            bool result = await _repos.SetRole(id,role);
            return result ? NoContent() : BadRequest();
        }

        [HttpPut("updateuser/{id}")]
        public async Task<ActionResult> PutUser(string id,UserVM userVM)
        {
            bool result = await _repos.PutUser(id,userVM);
            return result ? NoContent() : BadRequest();
        }

        [HttpPost("resetpassword")]
        public async Task<ActionResult> ResetPassword([FromQuery]string email,[FromQuery]string password)
        {
            bool result = await _repos.ResetPassword(email, password);

            return result ? NoContent() : BadRequest();
        }

        private string GenerateToken(LoginVM loginVM,string role)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginVM.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier,loginVM.Email),
                new Claim(ClaimTypes.Role,role)
               };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
    }
}
