using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.CustomerRequest
{
    public class CustomerRequestFilter
    {


        public CustomerRequestStatus? Status { get; set; }

        public DateTime? ExctionDate { get; set; }

        public Guid? StaffId { get; set; }

        public Guid? OrderId { get; set; }

        public Guid? UserId { get; set; }
    }
}
