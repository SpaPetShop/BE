using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Transaction
{
    public Guid Id { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public string? InvoiceCode { get; set; }

    public double? TotalAmount { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? PayType { get; set; }

    public string? TransactionJson { get; set; }

    public Guid? PaymentId { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? AccountId { get; set; }

    public virtual Payment? Payment { get; set; }
}
