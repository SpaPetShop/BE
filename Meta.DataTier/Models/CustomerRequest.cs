using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class CustomerRequest
{
    public Guid Id { get; set; }

    public string? Note { get; set; }

    public string? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ExctionDate { get; set; }

    public Guid? StaffId { get; set; }

    public Guid? OrderId { get; set; }

    public Guid? UserId { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Account? Staff { get; set; }

    public virtual Account? User { get; set; }
}
