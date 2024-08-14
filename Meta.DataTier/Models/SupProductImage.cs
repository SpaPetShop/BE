using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class SupProductImage
{
    public Guid Id { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? SupProductId { get; set; }

    public virtual SupProduct? SupProduct { get; set; }
}
