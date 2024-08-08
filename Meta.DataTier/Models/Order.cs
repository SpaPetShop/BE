﻿using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Order
{
    public Guid Id { get; set; }

    public string? InvoiceCode { get; set; }

    public string? Status { get; set; }

    public string? Note { get; set; }

    public double? TotalAmount { get; set; }

    public string? Description { get; set; }

    public double? FinalAmount { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public Guid? AccountId { get; set; }

    public Guid? PetId { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Pet? Pet { get; set; }

    public virtual ICollection<TaskManager> TaskManagers { get; set; } = new List<TaskManager>();
}
