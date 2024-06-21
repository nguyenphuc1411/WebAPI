using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PFood.Context;
using PFood.Data;
using PFood.Models;
using System.Reflection.Metadata.Ecma335;
using static System.Net.Mime.MediaTypeNames;

namespace PFood.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly FoodDbContext _context;
        private readonly IMapper _mapper;

        public CategoryRepository(FoodDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> CreateAsync(CategoryVM categoryVM)
        {
           var category= _mapper.Map<Category>(categoryVM);
            _context.Categories.Add(category);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var cateDelete = await _context.Categories.FindAsync(id);
            if (cateDelete != null)
            {
                _context.Categories.Remove(cateDelete);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;

        }

        public async Task<List<CategoryVM>> GetAllAsync()
        {
            try
            {
                var listCategory = await _context.Categories.ToListAsync();
                return _mapper.Map<List<CategoryVM>>(listCategory);
            }
            catch
            {
                return new List<CategoryVM>();
            }
        }

        public async Task<CategoryVM> GetAsync(int id)
        {
            var cate = await _context.Categories.FindAsync(id);
            if (cate != null)
            {
                var cateVM = new CategoryVM
                {
                    CategoryID = cate.CategoryID,
                    Name = cate.Name,
                    Image = cate.Image
                };
                return cateVM;
            }

            return new CategoryVM();

        }

        public async Task<bool> PutAsync(int id, CategoryVM category)
        {
            if(id!= category.CategoryID)
            {
                return false;
            }
            var isExist = await _context.Categories.FindAsync(id);
            if (isExist != null)
            {
                isExist.Name = category.Name;
                isExist.Image = category.Image;
                var result= await _context.SaveChangesAsync();
                return result > 0;
            }
            return false;
        }
    }
}
