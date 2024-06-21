using PFood.Models;

namespace PFood.Services
{
    public interface ICouponRepository
    {
        Task<List<CouponVM>> GetAsync();
        Task<CouponVM> GetByIdAsync(int id);
        Task<CouponVM> GetByCodeAsync(string code);
        Task<bool> CreateAsync(CouponVM couponVM);
        Task<bool> UpdateAsync(int id, CouponVM couponVM);
        Task<bool> DeleteAsync(int id);
    }
}
