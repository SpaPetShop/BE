using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Enums.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Category
{
    public class CategoryFilter
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public CategoryStatus? Status { get; set; }
        public CategoryType? Type { get; set; }
        public Guid? MasterCategoryId { get; set; }
    }
}
