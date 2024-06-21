namespace PFood.Models
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string UserID { get; set; }
        public string Token { get; set; }
        public string Role { get; set; }
    }
}
