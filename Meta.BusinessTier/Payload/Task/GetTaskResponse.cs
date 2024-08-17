
using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Enums.EnumTypes;
using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Payload.Order;
using Meta.BusinessTier.Payload.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Task
{
    public class GetTaskResponse
    {
        public Guid Id { get; set; }

        public TaskType? Type { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? ExcutionDate { get; set; }

        public TaskManagerStatus? Status { get; set; }

        public DateTime? CompletedDate { get; set; }


        public OrderResponse? Order { get; set; }

        public AccountResponse? Staff { get; set; }
        public OrderPetResponse? Pets { get; set; }
    }
    public class OrderResponse
    {
        public Guid? Id { get; set; }
        public string? InvoiceCode { get; set; }
        public double? FinalAmount { get; set; }
    }
    public class AccountResponse
    {
        public Guid? Id { get; set; }
        public string? FullName { get; set; }
        public RoleEnum? Role { get; set; }
    }
}
