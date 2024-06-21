using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Models
{
    public class FoodVM
    {
        public int? FoodID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Discount { get; set; }
        [Required, Column(TypeName = "varchar(255)")]
        public string ImageMain { get; set; }   
        public string? ImageFirst { get; set; }     
        public string? ImageSecond { get; set; }    
        public string? ImageThree { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public int CategoryID { get; set; }
    }
}
