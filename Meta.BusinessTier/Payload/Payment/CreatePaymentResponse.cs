﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Payment
{
    public class CreatePaymentResponse
    {
        public string? Message { get; set; }
        public string? Url { get; set; }

        public Guid? PaymentId { get; set; }
    }
}
