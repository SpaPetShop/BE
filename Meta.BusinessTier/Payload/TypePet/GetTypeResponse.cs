using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.TypePet

{
    public class GetTypeResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public TypePetStatus Status { get; set; }
        public TypePetType Type { get; set; }
        public Guid? MasterCategoryId { get; set; }

    }
}
