﻿using Meta.BusinessTier.Payload.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<CreatePaymentResponse> ExecutePayment(CreatePaymentRequest request);
        Task<bool> ExecuteVnPayCallback(string? status, string? transId, string? urlCallBack);

        Task<bool> UpdatePayment(Guid id, UpdatePaymentRequest updatePaymentRequest);

        Task<ICollection<GetPaymentResponse>> GetPaymentList(PaymentFilter filter);
        Task<GetPaymentResponse> GetPaymentById(Guid id);
    }
}
