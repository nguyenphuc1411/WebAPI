using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PFood.Data
{
    public class Coupon
    {
        [Key]
        public int CouponID { get; set; }
        [Required, Column(TypeName = "varchar(20)")]
        public string CouponCode { get; set; }
        [Required]
        public DateTime ExpiryDate { get; set; }
        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Discount { get; set; }
        [Required, Column(TypeName = "nvarchar(20)")]
        public string Status { get; set; }
        public virtual Order Order { get; set; }
    }
}
