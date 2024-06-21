using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Data
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }
        [Required]
        public int Rating { get; set; }
        [Column(TypeName = "nvarchar(255)")]
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string UserID { get; set; }
        public virtual User User { get; set; }
        public int FoodID { get; set; }
        public virtual Food Food { get; set; }
    }
}
