using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
	public interface IReviewRepository
	{
		Task<bool> PostAsync(ReviewVM reviewVM);
		Task<List<ReviewVM>> GeyByFoodAsync(int foodID);
		Task<bool> DeleteAsync(int id);
	}
}
