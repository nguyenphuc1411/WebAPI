using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PFood.Context;
using PFood.Data;

namespace PFood.Services
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly FoodDbContext _context;

        public FeedbackRepository(FoodDbContext context)
        {
            _context = context;
        }

        public async Task<List<Feedback>> Get()
        {          
            return await _context.Feedbacks.ToListAsync();
        }

        public async Task<bool> Post(Feedback feedback)
        {
            _context.Feedbacks.Add(feedback);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}
