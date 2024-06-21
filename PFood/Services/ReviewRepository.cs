using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PFood.Context;
using PFood.Data;
using PFood.Models;
using System.Net.WebSockets;

namespace PFood.Services
{
	public class ReviewRepository : IReviewRepository
	{
		private readonly FoodDbContext _context;
		private readonly IMapper _mapper;
		public ReviewRepository(FoodDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;
		}


		public Task<bool> DeleteAsync(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<List<ReviewVM>> GeyByFoodAsync(int foodID)
		{
			var listReviews = await _context.Reviews
			.Where(x => x.FoodID ==foodID ).Select(review =>
				new ReviewVM
				{
					Rating = review.Rating,
					Comment = review.Comment,
					FoodID = foodID,
					UserID = review.UserID,
					Fullname = review.User.Fullname
				}
			).ToListAsync();
			return listReviews;
		}

		public async Task<bool> PostAsync(ReviewVM reviewVM)
		{
			_context.Reviews.Add(_mapper.Map<Review>(reviewVM));
			var result = await _context.SaveChangesAsync();
			return result > 0;
		}
	}
}
