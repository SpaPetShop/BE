
using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Enums.EnumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Task
{
    public class TaskFilter
    {

        public TaskType? Type { get; set; }

        public TaskManagerStatus? Status { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? AccountId { get; set; }

        public DateTime? ExcutionDate { get; set; }
    }
}
