using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.CustomerRequest
{
    public class CreateNewCustomerRequest
    {
        public string? Note { get; set; }

        public DateTime? ExctionDate { get; set; }

        public Guid? StaffId { get; set; }

        public Guid? OrderId { get; set; }
    }
}
