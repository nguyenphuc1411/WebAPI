using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PFood.Models
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Fullname is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers.")]
        public string Fullname { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }      
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Required]
        [Compare("Password",ErrorMessage ="Do not match Password")]
        public string ConfirmPassword {  get; set; }
    }
}
