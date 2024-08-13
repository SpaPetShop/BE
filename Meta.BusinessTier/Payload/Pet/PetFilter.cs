using Meta.BusinessTier.Payload.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Pet
{
    public class PetFilter
    {
        public string? Name { get; set; }

        public double? Weight { get; set; }

        public int? Age { get; set; }

        public Guid? TypePetId { get; set; }

        public Guid? CustomerId { get; set; }
    }
}
