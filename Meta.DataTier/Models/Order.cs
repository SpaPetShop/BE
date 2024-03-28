using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public string? InvoiceCode { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public string? Note { get; set; }

    public double? TotalAmount { get; set; }

    public double? Discount { get; set; }

    public double? FinalAmount { get; set; }

    public Guid? UserId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual User? User { get; set; }
}
