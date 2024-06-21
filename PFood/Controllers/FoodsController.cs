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
    [Authorize(Roles = "Admin")]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodRepository _repos;

        public FoodsController(IFoodRepository repos)
        {
            _repos = repos;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<FoodVM>>> Get()
        {
            return Ok(await _repos.GetAsync());
        }
        [HttpGet("news")]
        [AllowAnonymous]
        public async Task<ActionResult<List<FoodVM>>> GetNew()
        {
            return Ok(await _repos.GetNew());
        }
        [HttpGet("bestselling")]
        [AllowAnonymous]
        public async Task<ActionResult<FoodVM>> GetBestSelling()
        {
            return Ok(await _repos.GetBestDeal());
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Food>> Get(int id)
        {
            var food = await _repos.GetByIdAsync(id);
            if(food == null) return NotFound();
            return Ok(food);
        }
        [HttpPost]
        public async Task<ActionResult> Post(FoodVM foodVM)
        {
            bool result = await _repos.CreateAsync(foodVM);
            return result ? NoContent() : BadRequest();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id,FoodVM foodVM)
        {
            if(id!= foodVM.FoodID) return BadRequest();
            var result = await _repos.UpdateAsync(id,foodVM);
            return result ? NoContent() : BadRequest();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            bool result = await _repos.DeleteAsync(id);
            return result ? NoContent() : BadRequest();
        }
    }
}
