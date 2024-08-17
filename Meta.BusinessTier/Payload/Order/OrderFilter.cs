using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
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
        public DateTime? CreateDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public Guid? NoteId { get; set; }
        public string? Description { get; set; }
        public OrderStatus? Status { get; set; }
        public OrderType? Type { get; set; }
        public Guid? AccountId { get; set; }
        public Guid? PetId { get; set; }
    }
}
