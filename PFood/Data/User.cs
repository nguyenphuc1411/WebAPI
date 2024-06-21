using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PFood.Data
{
    public class User:IdentityUser
    {
        [Required, Column(TypeName = "nvarchar(150)")]
        public string Fullname { get; set; }
        [Column(TypeName = "varchar(13)")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8,9}$", ErrorMessage = "Invalid phone number VietNam format. Phone number must be 10 or 11 digits long and start with 03, 05, 07, 08, or 09.")]
        [MaxLength(11, ErrorMessage = "Phone number only 10-11 chars")]
        public string? PhoneNumber { get; set; }
        public DateTime? DOB { get; set; }     
        [Column(TypeName = "Varchar(255)")]
        public string? Address { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? Avartar { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.Now;
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
