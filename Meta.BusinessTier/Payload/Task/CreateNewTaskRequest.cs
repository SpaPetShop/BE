using Meta.BusinessTier.Enums.EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Task
{
    public class CreateNewTaskRequest
    {
        public Guid OrderId { get; set; }
        public Guid AccountId { get; set; }

    }
}
