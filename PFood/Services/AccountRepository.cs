
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PFood.Context;
using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public class AccountRepository : IAccountRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        public AccountRepository(UserManager<User> userManager, SignInManager<User> signInManager, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<bool> Delete(string id)
        {
            var user  = await _userManager.FindByIdAsync(id);
            if (user == null) { return false; }
            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<UserVM> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if(user == null) { return new UserVM(); }
            var roles = await _userManager.GetRolesAsync(user);
            var userVM = new UserVM
            {
                Id = user.Id,
                Fullname = user.Fullname,
                Address = user.Address,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                DOB = user.DOB,
                Avartar = user.Avartar,
                JoinedDate = user.JoinedDate,
                Role = roles[0]
            };
            return userVM;
        }

        public async Task<List<UserVM>> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
           
            var userVMs = new List<UserVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userVMs.Add(new UserVM
                {
                    Id = user.Id,
                    Fullname = user.Fullname,
                    Address = user.Address,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    DOB = user.DOB,
                    Avartar = user.Avartar,
                    JoinedDate = user.JoinedDate,
                    Role = roles[0]
                });
            }

            return userVMs;
        }

        public async Task<bool> IsExistsEmail(string email)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(x=> x.Email == email);

           return user != null;
        }

        public async Task<LoginResponse> Login(LoginVM loginVM)
        {
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user.UserName, loginVM.Password, false, false);
                if(result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    string role = roles[0];
                    return new LoginResponse
                    {
                        Success = true,
                        UserID = user.Id,
                        Role = role
                    };
                }
                else
                {
                    return new LoginResponse { Success = false };
                }
            }
            else
            {
                return new LoginResponse { Success = false };
            }
           
        }

        public async Task<bool> PutUser(string id , UserVM userVM)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                user.Email = userVM.Email;
                user.Fullname = userVM.Fullname;
                user.Address = userVM.Address;
                user.PhoneNumber = userVM.PhoneNumber;
                user.Avartar = userVM.Avartar;
                user.DOB = userVM.DOB;
               var resutl = await  _userManager.UpdateAsync(user);
                return resutl.Succeeded;
            }
            return false;
        }

        public async Task<bool> Register(RegisterVM registerVM)
        {
            var user = new User { Fullname = registerVM.Fullname, UserName = registerVM.Email, Email = registerVM.Email };
            var result = await _userManager.CreateAsync(user, registerVM.Password);         
            if (result.Succeeded)
            {
                var result1 = await _userManager.AddToRoleAsync(user,"Customer");
                if (result1.Succeeded)
                return true;
                else return false;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ResetPassword(string email, string newPassword)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                return result.Succeeded;
            }
            return false;
        }


        public async Task<bool> SetRole(string id, string role)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return false;
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var roleItem in roles)
            {
                await _userManager.RemoveFromRoleAsync(user, roleItem);
            }
            var result = await _userManager.AddToRoleAsync(user,role);
            return result.Succeeded;
        }

    }
}
