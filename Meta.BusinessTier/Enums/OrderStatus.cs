using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Enums
{
    public enum OrderStatus
    { 
        PENDING,
        CONFIRMED,
        PAID,
        COMPLETED,
        CANCELED

    }

    public enum OrderHistoryStatus
    {
        PENDING,
        CONFIRMED,
        PAID,
        COMPLETED,
        CANCELED
    }
}
