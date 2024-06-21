using AutoMapper;
using PFood.Data;
using PFood.Models;

namespace PFood.Mapper
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Category,CategoryVM>().ReverseMap();
            CreateMap<Food, FoodVM>().ReverseMap();
            CreateMap<Coupon, CouponVM>().ReverseMap();
            CreateMap<OrderVM, Order>()
            .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetailVMs)).ReverseMap();
            CreateMap<OrderDetail, OrderDetailVM>().ReverseMap();
			CreateMap<Review, ReviewVM>().ReverseMap();
            CreateMap<User, UserVM>().ReverseMap();
        }
    }
}
