using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PFood.Models;

namespace PFood.Data
{
    public class OrderDetail
    {
        [Required]
        [Column(TypeName ="decimal(10,2)")]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        public int FoodID { get; set; }
        public virtual Food Food { get; set; }
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
    }
}
