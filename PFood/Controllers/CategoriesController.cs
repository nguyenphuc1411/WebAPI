using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFood.Models;
using PFood.Services;

namespace PFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repos;

        public CategoriesController(ICategoryRepository repos)
        {
            _repos = repos;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<CategoryVM>>> Get()
        {           
            return Ok(await _repos.GetAllAsync());
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CategoryVM>> Get(int id)
        {
            return Ok(await _repos.GetAsync(id));
        }
        [HttpPost]
        public async Task<ActionResult> Post(CategoryVM category)
        {
            var result=await _repos.CreateAsync(category);
            return result ? Ok(result) : BadRequest();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id,CategoryVM category)
        {
            var result = await _repos.PutAsync(id,category);
            return result ? NoContent() : BadRequest();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _repos.DeleteAsync(id);
            return result ? NoContent() : BadRequest();
        }
    }
}
