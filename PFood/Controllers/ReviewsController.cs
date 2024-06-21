using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFood.Data;
using PFood.Models;
using PFood.Services;

namespace PFood.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    public class ReviewsController : ControllerBase
	{
		private readonly IReviewRepository _repos;

		public ReviewsController(IReviewRepository repos)
		{
			_repos = repos;
		}

		[HttpGet("{foodID}")]
		public async Task<ActionResult<List<ReviewVM>>> Get(int foodID)
		{
			var result = await _repos.GeyByFoodAsync(foodID);
			return Ok(result);
		}
		[HttpPost]
		[Authorize(Roles ="Admin,Customer")]
		public async Task<ActionResult> Post(ReviewVM reviewVM)
		{
			bool result =  await _repos.PostAsync(reviewVM);
			return result ? NoContent() : BadRequest();
		}
	}
}
