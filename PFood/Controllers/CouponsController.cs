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
    public class CouponsController : ControllerBase
    {
        private readonly ICouponRepository _repos;

        public CouponsController(ICouponRepository repos)
        {
            _repos = repos;
        }
        [HttpGet]
        public async Task<ActionResult<List<CouponVM>>> Get()
        {
            return Ok(await _repos.GetAsync());
        }
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CouponVM>> Get(int id)
        {
            var couponVM = await _repos.GetByIdAsync(id);
            if (couponVM == null) return NotFound();
            return Ok(couponVM);
        }
        [HttpGet("code/{code}")]
        [AllowAnonymous]
        public async Task<ActionResult<CouponVM>> Get(string code)
        {
            var couponVM = await _repos.GetByCodeAsync(code);
            if (couponVM == null) return NotFound();
            return Ok(couponVM);
        }
        [HttpPost]
        public async Task<ActionResult> Post(CouponVM couponVM)
        {
            bool result = await _repos.CreateAsync(couponVM);
            return result ? NoContent() : BadRequest();
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, CouponVM couponVM)
        {
            if (id != couponVM.CouponID) return BadRequest();
            var result = await _repos.UpdateAsync(id, couponVM);
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
