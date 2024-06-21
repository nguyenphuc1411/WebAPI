using static Azure.Core.HttpHeader;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Data
{
    public class Order
    {
        [Key]
        public int OrderID { get; set; }
        [Required, Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
        [Required, Column(TypeName = "varchar(13)")]
        public string PhoneNumber { get; set; }
        [Required, Column(TypeName = "varchar(50)")]
        public string Status { get; set; } = "";
        [Required, Column(TypeName = "nvarchar(255)")]
        public string DeliveryAddress { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string? Note {  get; set; }
        public DateTime OrderDate { get; set; }
        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        public string UserID { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public int? CouponID { get; set; }
        public virtual Coupon Coupon { get; set; }
        public virtual Payment Payment { get; set; }
    }
}
