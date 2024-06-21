using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public interface IFoodRepository
    {
        Task<List<FoodVM>> GetAsync();
        Task<Food> GetByIdAsync(int id);
        Task<bool> CreateAsync(FoodVM food);
        Task<bool> UpdateAsync(int id,FoodVM food);
        Task<bool> DeleteAsync(int id);
        Task<List<FoodVM>> GetNew();
        Task<FoodVM> GetBestDeal();
    }
}
