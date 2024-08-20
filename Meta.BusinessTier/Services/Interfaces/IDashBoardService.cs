using Meta.BusinessTier.Payload.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Interfaces
{
    public interface IDashBoardService
    {
        Task<CountAccount> CountAllAccount();

        Task<AdminDashboardStatistics> GetYearlyStatistics(int year);
        Task<CountOrders> CountAllOrde();
    }
}
