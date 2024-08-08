using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class TaskManager
{
    public Guid Id { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? CompletedDate { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual Order? Order { get; set; }
}
