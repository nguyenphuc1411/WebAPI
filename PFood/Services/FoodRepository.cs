using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PFood.Context;
using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public class FoodRepository : IFoodRepository
    {
        private readonly FoodDbContext _context;
        private readonly IMapper _mapper;
        public FoodRepository(FoodDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(FoodVM food)
        {
            var newFood = _mapper.Map<Food>(food);
            _context.Foods.Add(newFood);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var deleteFood=await  _context.Foods.FindAsync(id);
            if(deleteFood != null)
            {
                _context.Foods.Remove(deleteFood);
                var result= await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }

        public async Task<List<FoodVM>> GetAsync()
        {
            var listFood = await _context.Foods.ToListAsync();

            return _mapper.Map<List<FoodVM>>(listFood);
        }

        public async Task<Food> GetByIdAsync(int id)
        {
            var food = await _context.Foods.FindAsync(id);
            if(food != null)
            {
                return food;
            }
            return new Food();
        }

        public async Task<List<FoodVM>> GetNew()
        {
            var list = await _context.Foods.OrderByDescending(x => x.CreatedDate).Where(x=>x.Status== true).Take(4).ToListAsync();
            return _mapper.Map<List<FoodVM>>(list);
        }

        public async Task<bool> UpdateAsync(int id, FoodVM foodVM)
        {
            if (id != foodVM.FoodID) return false;
            var updateFood = await _context.Foods.FindAsync(id);
            if(updateFood == null) { return false; }
            _mapper.Map(foodVM,updateFood);
            _context.Entry(updateFood).State = EntityState.Modified;
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        public async Task<FoodVM> GetBestDeal()
        {
            var bestSelling = await _context.OrderDetails.GroupBy(od => od.FoodID)
                .OrderByDescending(sold => sold.Sum(x => x.Quantity)).Select( od=>
                    new { FoodID = od.Key}
                ).FirstOrDefaultAsync();
            if(bestSelling != null)
            {
                var foodBestSelling = await _context.Foods.Where(x => x.FoodID == bestSelling.FoodID).FirstOrDefaultAsync();
                return _mapper.Map<FoodVM>(foodBestSelling);
            }
            return new FoodVM();
        }
    }
}
