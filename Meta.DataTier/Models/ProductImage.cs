using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class ProductImage
{
    public Guid Id { get; set; }

    public string? ImageUrl { get; set; }

    public Guid? ProductId { get; set; }

    public virtual Product? Product { get; set; }
}
