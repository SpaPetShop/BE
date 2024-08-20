using Meta.BusinessTier.Enums.Status;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Payload.Dashboard
{
    public class DashBoard
    {
    }
    public class CountAccount
    {
        public int TolalAccount { get; set; }
        public Dictionary<UserStatus, int>? AccountByStatus { get; set; }
        public Dictionary<RoleEnum, int>? AccountByRole { get; set; }

    }
    public class AdminDashboardStatistics
    {
        public int TotalOrders { get; set; }
        public Dictionary<OrderStatus, int>? OrdersByStatus { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalProfit { get; set; }
        public List<MonthlyStatistics> MonthlyStatistics { get; set; }
    }
    public class MonthlyStatistics
    {
        public int Month { get; set; }
        public int TotalOrders { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalProfit { get; set; }
    }
    public class CountOrders
    {
        public int TolalOrders { get; set; }
        public Dictionary<OrderStatus, int>? OrdersByStatus { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalProfit { get; set; }

    }
}
