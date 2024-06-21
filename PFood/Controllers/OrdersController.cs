using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PFood.Context;
using PFood.Data;
using PFood.Models;
using PFood.Services;

namespace PFood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Customer")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _reposs;

        public OrdersController(IOrderRepository reposs)
        {
            _reposs = reposs;
        }
        [HttpGet]
        public async Task<ActionResult<List<OrderVM>>> Get([FromQuery] string? fullname, [FromQuery] string? status)
        {
            var result = await _reposs.GetAllAsync(fullname, status);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> Get(int id)
        {
            var order = await _reposs.GetByIdAsync(id);
            if(order == null) return NotFound();
            return order;
        }
        [HttpPost]
        public async Task<ActionResult> Post(OrderVM orderVM)
        {
            bool result = await _reposs.PostAsync(orderVM);
            return result ? Ok() : BadRequest();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<List<Order>>> Delete(int id)
        {
            bool result = await _reposs.DeleteAsync(id);        
            return result ? NoContent() : BadRequest();
        }
        [HttpPut("update-status")]
        public async Task<IActionResult> UpdateStatus([FromQuery] int id, [FromQuery] string status)
        {
            if (id <= 0 || string.IsNullOrEmpty(status))
            {
                return BadRequest("Invalid parameters.");
            }

            bool result = await _reposs.UpdateStatusAsync(id, status);
            return result ? NoContent() : BadRequest("Failed to update order status.");
        }
    }
}
