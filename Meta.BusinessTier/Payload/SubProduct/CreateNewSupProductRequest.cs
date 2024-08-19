using Meta.BusinessTier.Enums.Status;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.PetService
{
    public class CreateNewSupProductRequest
    {
        public string Name { get; set; }
        public double? StockPrice { get; set; }
        public double? SellingPrice { get; set; }
        public string Description { get; set; }
        public ProductStatus Status { get; set; }
        public Guid CategoryId { get; set; }
        public double? TimeWork { get; set; }
        public int Priority { get; set; }
        public List<Images>? Image { get; set; } = new List<Images>();
    }
    public class Images
    {
        public string? ImageURL { get; set; }

    }
}
