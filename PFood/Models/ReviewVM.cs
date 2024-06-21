using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PFood.Models
{
	public class ReviewVM
	{
		public int ReviewID { get; set; }
		[Required]
		public int Rating { get; set; }	
		public string Comment { get; set; }
		
		[Required]
		public int FoodID { get; set; }
		public DateTime CreatedDate { get; set; } = DateTime.Now;
		public string UserID { get; set; }
		public string? Fullname { get; set; }
	}
}
