using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class TypePet
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public Guid? MasterTypeId { get; set; }

    public virtual ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
