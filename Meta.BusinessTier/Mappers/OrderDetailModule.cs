using AutoMapper;
using Meta.BusinessTier.Payload.Order;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Mappers
{
    public class OrderDetailModule : Profile
    {
        public OrderDetailModule()
        {
            CreateMap<Order, GetOrderDetailResponse>();
        }
    }
}
