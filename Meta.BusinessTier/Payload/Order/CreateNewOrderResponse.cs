using Meta.BusinessTier.Enums.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Order
{
    public class CreateNewOrderResponse
    {
        public List<OrderProduct> ProductList { get; set; } = new List<OrderProduct>();
        public DateTime ExcutionDate { get; set; }
        public string? Note { get; set; }
        public string? Description { get; set; }
        public OrderType Type { get; set; }
        public Guid PetId { get; set; }
        public Guid? StaffId { get; set; }






    }
    public class OrderProduct
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public double TimeWork { get; set; }
        public float SellingPrice { get; set; }
    }
    public class OrderSupProduct
    {
        public Guid SupProductId { get; set; }
        public int Quantity { get; set; }
        public double TimeWork { get; set; }
        public float SellingPrice { get; set; }
    }
}
