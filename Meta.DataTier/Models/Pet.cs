using System;
using System.Collections.Generic;

namespace Meta.DataTier.Models;

public partial class Pet
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public double? Weight { get; set; }

    public int? Age { get; set; }

    public string? Image { get; set; }

    public Guid? TypePetId { get; set; }

    public Guid? AccountId { get; set; }

    public virtual Account? Account { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual TypePet? TypePet { get; set; }
}
