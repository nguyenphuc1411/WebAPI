using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Models
{
    public class CouponVM
    {
        public int CouponID { get; set; }
        [Required]
        public string CouponCode { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required]
        public decimal Discount { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
