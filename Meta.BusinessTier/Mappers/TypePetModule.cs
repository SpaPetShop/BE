using AutoMapper;
using Meta.BusinessTier.Payload.TypePet;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Mappers
{
    public class TypePetModule : Profile
    {
        public TypePetModule() {
            CreateMap<TypePet, GetTypeResponse>();
            CreateMap<CreateNewTypeRequest, TypePet>();
        }
    }
}
