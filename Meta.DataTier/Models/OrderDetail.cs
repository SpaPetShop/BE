using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class OrderDetail
{
    public Guid Id { get; set; }

    public double? TotalAmount { get; set; }

    public int? Quantity { get; set; }

    public double? SellingPrice { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? ProductId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Product? Product { get; set; }
}
