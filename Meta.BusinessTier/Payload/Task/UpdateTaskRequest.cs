using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Task
{
    public class UpdateTaskRequest
    {

        public Guid? AccountId { get; set; }
        public DateTime? ExcutionDate { get; set; }
    }
}
