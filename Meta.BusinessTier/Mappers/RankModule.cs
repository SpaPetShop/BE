using AutoMapper;
using Meta.BusinessTier.Payload.Order;
using Meta.BusinessTier.Payload.Rank;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Mappers
{
    public class RankModule : Profile
    {
        public RankModule() {
            CreateMap<CreateNewRankRequest, Rank>();
        }
    }
}
