using Microsoft.AspNetCore.Components.Web;
using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public interface ICategoryRepository
    {
        Task<List<CategoryVM>> GetAllAsync();
        Task<CategoryVM> GetAsync(int id);
        Task<bool> CreateAsync(CategoryVM category);
        Task<bool> PutAsync(int id,CategoryVM category);
        Task<bool> DeleteAsync(int id);
    }
}
