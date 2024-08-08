using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Order
{
    public class UpdateOrderRequest
    {
        public OrderStatus Status { get; set; }
        public string Note { get; set; }
    }
}
