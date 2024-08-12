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

        public string? Note { get; set; }

        public string? Description { get; set; }

        public Guid PetId { get; set; }

        public Guid AccountId { get; set; }



    }
    public class OrderProduct
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public float SellingPrice { get; set; }
        public double TotalAmount { get; set; }
    }
    
}
