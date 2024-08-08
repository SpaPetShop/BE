using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Rank
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public int? Range { get; set; }
}
