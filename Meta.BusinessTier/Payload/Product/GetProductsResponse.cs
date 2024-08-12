using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Product
{
    public class GetProductsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double? StockPrice { get; set; }
        public double? SellingPrice { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        public int? Priority { get; set; }
        public CategoryResponse Category { get; set; }
    }
    public class CategoryResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
