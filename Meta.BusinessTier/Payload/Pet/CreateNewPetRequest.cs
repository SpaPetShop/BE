using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Pet
{
    public class CreateNewPetRequest
    {
        public string? Name { get; set; }

        public double? Weight { get; set; }

        public int? Age { get; set; }

        public string? Image { get; set; }

        public Guid? TypePetId { get; set; }

    }
}
