using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Payment
{
    public class UpdatePaymentRequest
    {
        public PaymentStatus? Status { get; set; }

        public string? Note { get; set; }
    }
}
