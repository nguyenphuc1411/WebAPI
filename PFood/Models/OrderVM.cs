using System.ComponentModel.DataAnnotations;

namespace PFood.Models
{
    public class OrderVM
    {
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Full Name cannot contain numbers.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^(03|05|07|08|09)\d{8,9}$", ErrorMessage = "Invalid phone number VietNam format. Phone number must be 10 or 11 digits long and start with 03, 05, 07, 08, or 09.")]
        [MaxLength(11, ErrorMessage = "Phone number only 10-11 chars")]
        public string PhoneNumber { get; set; }

        public string? Status { get; set; }

        [Required]
        [MaxLength(255, ErrorMessage = "DeliveryAddress max length is 255 chars")]
        public string DeliveryAddress { get; set; }

        [MaxLength(255, ErrorMessage = "Note max length is 255 chars")]
        public string? Note { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        public decimal TotalAmount { get; set; }

        public string? UserID { get; set; }

        public int? CouponID { get; set; }

        public virtual ICollection<OrderDetailVM> OrderDetailVMs { get; set; } = new List<OrderDetailVM>();
    }
}
