using Meta.BusinessTier.Payload.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Pet
{
    public class GetPetResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public double? Weight { get; set; }

        public int? Age { get; set; }

        public string? Image { get; set; }

        public TypePetResponse? TypePet { get; set; }

        public AccountResponse? Customer { get; set; }
    }
    public class TypePetResponse{
        public Guid Id { get; set; }
        public string? Name { get; set; }

    }

}
