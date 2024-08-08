using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Order
{
    public class OrderFilter
    {
        public string? InvoiceCode { get; set; }
        public DateTime? fromDate { get; set; }
        public DateTime? toDate { get; set; }
        public OrderStatus? status { get; set; }
    }
}
