using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Data
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }
        [Required, Column(TypeName = "nvarchar(255)")]
        public string Name { get; set; }
        [Required, Column(TypeName = "varchar(255)")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, Column(TypeName = "varchar(13)")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
        [Required, Column(TypeName = "nvarchar(255)")]
        public string Subject { get; set; }
        [Required, Column(TypeName = "nvarchar(255)")]
        public string Message { get; set; }
    }
}
