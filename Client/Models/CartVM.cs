using PFood.Data;

namespace Client.Models
{
	public class CartVM
	{
		public int FoodID {  get; set; }
		public string Name {  get; set; }
		public string Image {  get; set; }
		public decimal Price {  get; set; }
		public int Quantity {  get; set; }
	}
}
