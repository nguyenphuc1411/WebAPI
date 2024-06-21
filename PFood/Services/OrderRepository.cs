using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using PFood.Context;
using PFood.Data;
using PFood.Models;

namespace PFood.Services
{
    public class OrderRepository : IOrderRepository
    {
        private readonly FoodDbContext _context;
        private readonly IMapper _mapper;
        public OrderRepository(FoodDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) { return false; }
            _context.Orders.Remove(order);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<List<OrderVM>> GetAllAsync(string? fullname, string? status)
        {
            var listOrder = _context.Orders.AsQueryable();
            if (!string.IsNullOrEmpty(fullname))
            {
                listOrder = listOrder.Where(x=>x.FullName.ToLower().Contains(fullname.ToLower()));
            }            
            if (!string.IsNullOrEmpty(status))
            {
                listOrder = listOrder.Where(x=>x.Status.ToLower()==status.ToLower());
            }
            var orders = await listOrder.ToListAsync();
            return _mapper.Map<List<OrderVM>>(orders);
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if(order != null)
            {
                var listDetail = await _context.OrderDetails.Where(x=>x.OrderID==order.OrderID).Select(x => new OrderDetail
                {
                    FoodID = x.FoodID,
                    OrderID = x.OrderID,
                    Quantity = x.Quantity,
                    Price = x.Price,
                    Food = x.Food
                }
                ).ToListAsync();
                order.OrderDetails = listDetail;               
                return order;
            }
            else
            {
                return new Order();
            }
            
        }

        public async Task<bool> PostAsync(OrderVM orderVM)
        {
            var listDetail = orderVM.OrderDetailVMs.ToList();

            var order = _mapper.Map<Order>(orderVM);

            _context.Orders.Add(order);

            var resultOrder = await _context.SaveChangesAsync();
            
            if(resultOrder > 0)
            {               
                if (orderVM.CouponID.HasValue)
                {
                    var coupon = await _context.Coupons.FindAsync(orderVM.CouponID);
                    if(coupon!= null)
                    {
                        coupon.Status = "Used";
                        _context.Coupons.Update(coupon);
                        var resultCoupon = await _context.SaveChangesAsync();
                        return resultCoupon > 0;
                    }
                    return true;
                }
                return true;
            }
            return false;
            
        }
        public async Task<bool> PutAsync(int id, OrderVM orderVM)
        {
            // chưa dùng đến
            var order = _mapper.Map<Order>(orderVM);
            _context.Orders.Add(order);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                foreach (var item in orderVM.OrderDetailVMs)
                {
                    item.OrderID = order.OrderID;
                    _context.OrderDetails.Add(_mapper.Map<OrderDetail>(item));
                }
                var result1 = await _context.SaveChangesAsync();
                return result1 > 0;
            }
            return false;
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            if(id  <= 0 || string.IsNullOrEmpty(status))
            {
                return false;
            }
            var order = await _context.Orders.FindAsync(id);
            if(order == null) { return false; }
            else
            {
                if(order.Status== "Wait for confirmation")
                {
                    order.Status = status;
                    _context.Orders.Update(order);
                    var result = await _context.SaveChangesAsync();
                    return result > 0;
                }
                return false;
            }
        }
    }
}
