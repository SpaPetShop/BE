
using Meta.BusinessTier.Payload;
using Meta.BusinessTier.Payload.Category;
using Meta.BusinessTier.Payload.Order;
using Meta.DataTier.Paginate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Guid> CreateNewOrder(CreateNewOrderResponse createNewOrderRequest);
        Task<GetOrderDetailResponse> GetOrderDetail(Guid id);
        Task<IPaginate<GetOrderResponse>> GetOrderList(OrderFilter filter, PagingModel pagingModel);
        Task<bool> UpdateOrder(Guid orderId, UpdateOrderRequest request);

        Task<IEnumerable<GetOrderHistoriesResponse>> GetOrderHistories(Guid orderId);
    }
}
