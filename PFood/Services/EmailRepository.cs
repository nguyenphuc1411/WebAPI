using PFood.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Cryptography;

namespace PFood.Services
{
    public class EmailRepository : IEmailRepository
    {
        private readonly IConfiguration _config;

        public EmailRepository(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(EmailVM emailVM)
        {
            try
            {
                var fromAddress = new MailAddress(_config["Smtp:FromEmail"], _config["Smtp:FromName"]);
                var toAddress = new MailAddress(emailVM.ToEmail);

                using var smtpClient = new SmtpClient
                {
                    Host = _config["Smtp:Host"],
                    Port = int.Parse(_config["Smtp:Port"]),
                    EnableSsl = bool.Parse(_config["Smtp:EnableSsl"]),
                    Credentials = new NetworkCredential(_config["Smtp:UserName"], _config["Smtp:Password"])
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = emailVM.Subject,
                    Body = emailVM.Body,
                    IsBodyHtml = true
                };

                await smtpClient.SendMailAsync(message);
                return true; // Gửi thành công
            }
            catch (SmtpException smtpEx)
            {
                Console.WriteLine($"SMTP Error sending email: {smtpEx.Message}");
                return false; // Gửi thất bại
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error sending email: {ex.Message}");
                return false; // Gửi thất bại
            }
        }

        public async Task<bool> SendEmaiResetPassword(string email,string code)
        {
            try
            {
                var fromAddress = new MailAddress(_config["Smtp:FromEmail"], _config["Smtp:FromName"]);
                var toAddress = new MailAddress(email);
                const string subject = "Password Reset Request";
                string body = GeneratePasswordResetEmailBody(code);
                using var smtpClient = new SmtpClient
                {
                    Host = _config["Smtp:Host"],
                    Port = int.Parse(_config["Smtp:Port"]),
                    EnableSsl = bool.Parse(_config["Smtp:EnableSsl"]),
                    Credentials = new NetworkCredential(_config["Smtp:UserName"], _config["Smtp:Password"])
                };

                using var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch
            {
                return false;
            }
           
        }

        private string GeneratePasswordResetEmailBody(string code)
        {
            var sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial, sans-serif; color: #333; }");
            sb.AppendLine(".container { width: 80%; margin: auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div class='container'>");
            sb.AppendLine("<h1>Password Reset Request</h1>");
            sb.AppendLine($"<p>We received a request to reset your password. Your confirmation code is: {code}</p>");
            sb.AppendLine("<p>If you did not request a password reset, please ignore this email.</p>");
            sb.AppendLine("<p>Thank you,</p>");
            sb.AppendLine("<p>Phuc Nguyen</p>");
            sb.AppendLine("</div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }
    }
}
