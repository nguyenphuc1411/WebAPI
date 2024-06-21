using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Models
{
    public class CategoryVM
    {
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "The name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Image is required")]
        public string Image { get; set; }
    }
}
