using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Product
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public Guid? CategoryId { get; set; }

    public virtual Category? Category { get; set; }
}
