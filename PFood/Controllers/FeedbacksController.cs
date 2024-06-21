using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFood.Data;
using PFood.Services;

namespace PFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackRepository _reposs;
        public FeedbacksController(IFeedbackRepository reposs)
        {
            _reposs = reposs;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<List<Feedback>>> Get()
        {
            return await _reposs.Get();
        }
        [HttpPost]
        public async Task<ActionResult> Post(Feedback feedback)
        {
            bool result = await _reposs.Post(feedback);
            return result ? NoContent() : BadRequest();
        }
    }
}
