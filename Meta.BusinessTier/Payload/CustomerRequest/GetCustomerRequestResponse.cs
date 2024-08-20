using Meta.BusinessTier.Payload.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.CustomerRequest
{
    public class GetCustomerRequestResponse
    {
        public Guid Id { get; set; }

        public string? Note { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? ExctionDate { get; set; }

        public AccountResponse? StaffId { get; set; }

        public Guid? OrderId { get; set; }

        public AccountResponse? UserId { get; set; }
    }

}
