using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public interface IAccountRepository
    {
        Task<LoginResponse> Login(LoginVM loginVM);
        Task<bool> Register(RegisterVM registerVM);
        Task<List<UserVM>> GetUsers();
        Task<UserVM> GetById(string id);
        Task<bool> PutUser(string id,UserVM userVM);
        Task<bool> Delete(string id);
        Task<bool> SetRole(string id,string role);
        Task<bool> IsExistsEmail (string email);
       Task<bool> ResetPassword (string email, string password);
    }
}
