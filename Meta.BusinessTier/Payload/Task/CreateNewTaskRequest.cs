using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Task
{
    public class CreateNewTaskRequest
    {

        public string? Type { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? AccountId { get; set; }

    }
}
