using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public interface IOrderRepository
    {
        Task<bool> PostAsync(OrderVM orderVM);
        Task<bool> PutAsync(int id, OrderVM orderVM);
        Task<bool> DeleteAsync(int id);
        Task<Order> GetByIdAsync(int id);
        Task<List<OrderVM>> GetAllAsync(string? fullname, string? status);
        Task<bool> UpdateStatusAsync(int id, string status);
    }
}
