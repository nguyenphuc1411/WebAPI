using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Data
{
    public class Food
    {
        [Key]
        public int FoodID { get; set; }
        [Required, Column(TypeName = "nvarchar(100)")]
        public string Name { get; set; }
        [Required, Column(TypeName = ("decimal(10,2)"))]
        public decimal Price { get; set; }
        [Required, Column(TypeName = "nvarchar(255)")]
        public string Description { get; set; }
        [Required, Column(TypeName = ("decimal(10,2)"))]
        public decimal Discount { get; set; }
        [Required,Column(TypeName = "varchar(255)")]
        public string ImageMain { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? ImageFirst { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? ImageSecond { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? ImageThree { get; set; }
        [Required]
        public bool Status {  get; set; }
        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
