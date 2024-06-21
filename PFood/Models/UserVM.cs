using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Models
{
    public class UserVM
    {
        public string? Id {  get; set; }
        [Required]
        [MaxLength(100)]
        public string Fullname { get; set; }
        [RegularExpression(@"^(03|05|07|08|09)\d{8,9}$", ErrorMessage = "Invalid phone number VietNam format. Phone number must be 10 or 11 digits long and start with 03, 05, 07, 08, or 09.")]
        [MaxLength(11, ErrorMessage = "Phone number only 10-11 chars")]
        public string? PhoneNumber { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        public DateTime? DOB { get; set; }
        [StringLength(255)]
        public string? Address { get; set; }
        [StringLength(255)]
        public string? Avartar { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public string? Role {  get; set; }
    }
}
