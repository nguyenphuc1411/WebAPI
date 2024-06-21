using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PFood.Context;
using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public class CouponRepository : ICouponRepository
    {
        private readonly FoodDbContext _context;
        private readonly IMapper _mapper;
        public CouponRepository(FoodDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<bool> CreateAsync(CouponVM couponVM)
        {
            var newCoupon = _mapper.Map<Coupon>(couponVM);
            _context.Coupons.Add(newCoupon);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleteCoupon = await _context.Coupons.FindAsync(id);
            if (deleteCoupon != null)
            {
                _context.Coupons.Remove(deleteCoupon);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        public async Task<List<CouponVM>> GetAsync()
        {
            var listCoupon = await _context.Coupons.ToListAsync();

            return _mapper.Map<List<CouponVM>>(listCoupon);
        }

        public async Task<CouponVM> GetByCodeAsync(string code)
        {
            var coupon = await _context.Coupons.SingleOrDefaultAsync(x => x.CouponCode == code && x.ExpiryDate > DateTime.Now && x.Status == "Active");
            return _mapper.Map<CouponVM>(coupon);
        }

        public async Task<CouponVM> GetByIdAsync(int id)
        {
            var coupon = await _context.Coupons.FindAsync(id);
            return _mapper.Map<CouponVM>(coupon);
        }

        public async Task<bool> UpdateAsync(int id, CouponVM couponVM)
        {
            if (id != couponVM.CouponID) return false;
            var updateCoupon = await _context.Coupons.FindAsync(id);
            if (updateCoupon == null) { return false; }
            _mapper.Map(couponVM, updateCoupon);
            _context.Entry(updateCoupon).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
