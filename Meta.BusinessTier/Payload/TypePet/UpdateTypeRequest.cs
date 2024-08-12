using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.TypePet

{
    public class UpdateTypeRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public TypePetStatus Status { get; set; }

    }
}
