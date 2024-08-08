using AutoMapper;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload.Product;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Mappers
{
    public class ProductModule : Profile
    {
        public ProductModule() {
            CreateMap<Product, GetProductsResponse>();
            CreateMap<CreateNewProductRequest, Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ProductStatus.Active.GetDescriptionFromEnum()));
        }
    }
}
