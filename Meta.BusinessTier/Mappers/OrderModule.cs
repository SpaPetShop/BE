using AutoMapper;
using Meta.BusinessTier.Enums;
using Meta.BusinessTier.Payload.Order;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Mappers
{
    public class OrderModule : Profile
    {
        public OrderModule()
        {
            CreateMap<Order, GetOrderResponse>();
            CreateMap<OrderHistory, GetOrderHistoriesResponse>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username));


        }
    }
}
