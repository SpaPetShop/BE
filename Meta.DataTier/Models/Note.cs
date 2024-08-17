using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Note
{
    public Guid Id { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? OrderId { get; set; }

    public virtual Order? Order { get; set; }
}
