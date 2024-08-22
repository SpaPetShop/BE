using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using Meta.BusinessTier.Payload.Pet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Order
{
    public class GetOrderDetailResponse
    {
        public Guid? OrderId { get; set; }
        public string? InvoiceCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public DateTime? ExcutionDate { get; set; }
        public string? Description { get; set; }
        public double? TotalAmount { get; set; }
        public double? FinalAmount { get; set; }
        public OrderStatus? Status { get; set; }
        public OrderType? Type { get; set; }
        public OrderUserResponse? UserInfo { get; set; }
        public OrderPetResponse? PetInfor { get; set; }
        public StaffResponse? Staff {  get; set; }
        public List<NoteResponse>? Note { get; set; } = new List<NoteResponse>();
        public List<OrderDetailResponse>? ProductList { get; set; } = new List<OrderDetailResponse>();
    }
    public class NoteResponse
    {
        public Guid? Id { get; set; }
        public NoteStatus? Status { get; set; }
        public string? Description { get; set; }
        public DateTime CreateDate { get; set; }

    }

    public class OrderDetailResponse
    {
        public Guid? OrderDetailId { get; set; }
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public Guid? SupProductId { get; set; }
        public string? SupProductName { get; set; }
        public int? Quantity { get; set; }
        public double? SellingPrice { get; set; }
        public double? TotalAmount { get; set; }

    }

    public class OrderUserResponse
    {
        public Guid? Id { get; set; }
        public string? FullName { get; set; }
        public string? Image { get; set; }
        public string? PhoneNumber { get; set; }
        public RoleEnum? Role { get; set; }
    }
    public class OrderPetResponse
    {
        public Guid? Id { get; set; }
        public string? Name { get; set; }
        public string? Image {  get; set; }
        public TypePetResponse? TypePet { get; set; }
    }
    public class StaffResponse
    {
        public Guid Id { get; set; }
        public string? FullName { get; set; }
        public string? Image { get; set; }
        public string? PhoneNumber { get; set; }
        public RoleEnum? Role { get; set;}
    }
}
