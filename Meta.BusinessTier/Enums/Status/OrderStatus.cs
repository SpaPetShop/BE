using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Enums.Status
{
    public enum OrderStatus
    {
        UNPAID,
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
