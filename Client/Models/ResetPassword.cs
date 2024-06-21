using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class ResetPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password {  get; set; }
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessage ="Password not match")]
        public string ConfirmPassword { get; set; }
    }
}
