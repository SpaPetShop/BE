using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Rank;
using Meta.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface IRankService 
    {
        Task<Guid> CreateNewRank(CreateNewRankRequest createNewRankRequest);
        Task<bool> UpdateRank(Guid id, UpdateRankRequest updateRankRequest);
        Task<ICollection<GetRankResponse>> GetRankList(RankFilter filter);
        Task<GetRankResponse> GetRankById(Guid id);
        Task<bool> RemoveRankStatus(Guid id);
        Task<IPaginate<GetAccountInforInRankResponse>> GetAccountInforInRank(Guid id, PagingModel pagingModel);

    }
}
