using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class ProductPetService
{
    public Guid Id { get; set; }

    public Guid? ProductId { get; set; }

    public Guid SupProductId { get; set; }

    public virtual Product? Product { get; set; }

    public virtual SupProduct SupProduct { get; set; } = null!;
}
