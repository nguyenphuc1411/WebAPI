using PFood.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Models
{
    public class OrderDetailVM
    {
        public int FoodID { get; set; }
        [Required]
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int OrderID { get; set; }
    }
}
