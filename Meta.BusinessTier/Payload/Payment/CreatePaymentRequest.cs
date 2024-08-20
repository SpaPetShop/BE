using Meta.BusinessTier.Enums.EnumTypes;
using Meta.BusinessTier.Enums.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Payment
{
    public class CreatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public double Amount { get; set; }
        public PaymentType PaymentType { get; set; }

        public string? CallbackUrl { get; set; }
        public Guid? AccountId { get; set; }
    }
}
