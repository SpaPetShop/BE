using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Rank
{
    public class GetRankResponse
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public int? Range { get; set; }
        public int? Value { get; set; }
    }
}
