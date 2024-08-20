using Meta.BusinessTier.Enums.Status;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.CustomerRequest
{
    public class UpdateCustomerRequest
    {
        public CustomerRequestStatus? Status { get; set; }
        public string? Note { get; set; }
    }
}
