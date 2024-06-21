using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Data
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        [Required(ErrorMessage = "The name is required")]
        [Column(TypeName = "Nvarchar(100)")]
        public string Name { get; set; }
        [Required(ErrorMessage = "The Image is required")]
        [Column(TypeName = "varchar(255)")]
        public string Image { get; set; }
        public virtual ICollection<Food> Foods { get; set; }
    }
}
