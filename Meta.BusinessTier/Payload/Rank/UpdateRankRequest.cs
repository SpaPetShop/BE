﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Rank
{
    public class UpdateRankRequest
    {
        public string? Name { get; set; }

        public int? Range { get; set; }
    }
}
