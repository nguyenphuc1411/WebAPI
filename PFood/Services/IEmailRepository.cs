using PFood.Models;

namespace PFood.Services
{
    public interface IEmailRepository
    {
        Task<bool> SendEmailAsync(EmailVM emailVM);

        Task<bool> SendEmaiResetPassword(string email,string code);
    }
}
