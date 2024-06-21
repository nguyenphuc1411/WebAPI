using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Data
{
    public class Payment
    {
        [Key]
        public int PaymentID { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required, Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }
        [Required, Column(TypeName = "Nvarchar(50)")]
        public string PaymentMethod { get; set; }
        public int OrderID { get; set; }
        public virtual Order Order { get; set; }
    }
}
