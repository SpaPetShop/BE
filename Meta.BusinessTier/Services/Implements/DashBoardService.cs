using AutoMapper;
using Meta.BusinessTier.Extensions;
using Meta.BusinessTier.Payload.Dashboard;
using Meta.BusinessTier.Services.Interfaces;
using Meta.DataTier.Models;
using Meta.DataTier.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Services.Implements
{
    public class DashBoardService : BaseService<DashBoardService>, IDashBoardService
    {
        public DashBoardService(IUnitOfWork<MetaContext> unitOfWork, ILogger<DashBoardService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
        }

        public async Task<CountAccount> CountAllAccount()
        {
            var account = await _unitOfWork.GetRepository<Account>().GetListAsync();

            return new CountAccount
            {
                AccountByRole = account.CountAccountEachRoleEnum(),
                AccountByStatus = account.CountAccountEachStatus(),
                TolalAccount = account.Count
            };
        }
        public async Task<AdminDashboardStatistics> GetYearlyStatistics(int year)
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetListAsync(
                predicate: o => o.CreatedDate.HasValue && o.CreatedDate.Value.Year == year,
                include : o => o.Include(o => o.OrderDetails));

            var paidOrCompletedOrders = orders
                .Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
                .ToList();

            double totalCost = paidOrCompletedOrders.Sum(o => o.OrderDetails.Sum(od => (od.Quantity ?? 0) * (od?.SellingPrice ?? 0)));
            double totalRevenue = paidOrCompletedOrders.Sum(o => o.FinalAmount ?? 0);
            double totalProfit = totalRevenue - totalCost;

            var monthlyStatistics = orders
                .GroupBy(o => o.CreatedDate.Value.Month)
                .Select(g => new MonthlyStatistics
                {
                    Month = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.FinalAmount ?? 0),
                    TotalProfit = g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.FinalAmount ?? 0) - g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.OrderDetails.Sum(od => (od.Quantity ?? 0) * (od?.SellingPrice ?? 0)))
                })
                .ToList();

            var completeMonthlyStatistics = Enumerable.Range(1, 12)
                .Select(month => monthlyStatistics.FirstOrDefault(ms => ms.Month == month) ?? new MonthlyStatistics
                {
                    Month = month,
                    TotalOrders = 0,
                    TotalRevenue = 0,
                    TotalProfit = 0
                })
                .OrderBy(ms => ms.Month)
                .ToList();

            return new AdminDashboardStatistics
            {
                TotalOrders = orders.Count,
                OrdersByStatus = orders.CountOrderEachStatus(),
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                MonthlyStatistics = completeMonthlyStatistics
            };
        }
        public async Task<AdminDashboardStatistics> GetMonthlyStatistics(int year, int month)
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetListAsync(
                predicate: o => o.CreatedDate.HasValue && o.CreatedDate.Value.Year == year && o.CreatedDate.Value.Month == month,
                include: o => o.Include(o => o.OrderDetails));

            var paidOrCompletedOrders = orders
                .Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
                .ToList();

            double totalCost = paidOrCompletedOrders.Sum(o => o.OrderDetails.Sum(od => (od.Quantity ?? 0) * (od?.SellingPrice ?? 0)));
            double totalRevenue = paidOrCompletedOrders.Sum(o => o.FinalAmount ?? 0);
            double totalProfit = totalRevenue - totalCost;

            // Thống kê theo tuần
            var weeklyStatistics = orders
                .GroupBy(o => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(o.CreatedDate.Value, CalendarWeekRule.FirstDay, DayOfWeek.Monday))
                .Select(g => new WeeklyStatistics
                {
                    Week = g.Key,
                    Year = year,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.FinalAmount ?? 0),
                    TotalProfit = g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.FinalAmount ?? 0) - g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.OrderDetails.Sum(od => (od.Quantity ?? 0) * (od?.SellingPrice ?? 0)))
                })
                .ToList();

            // Thống kê theo ngày và sắp xếp theo ngày gần nhất
            var dailyStatistics = orders
                .GroupBy(o => o.CreatedDate.Value.Date)
                .OrderByDescending(g => g.Key) // Sắp xếp theo ngày gần nhất
                .Select(g => new DailyStatistics
                {
                    Date = g.Key,
                    TotalOrders = g.Count(),
                    TotalRevenue = g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.FinalAmount ?? 0),
                    TotalProfit = g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.FinalAmount ?? 0) - g.Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase)).Sum(o => o.OrderDetails.Sum(od => (od.Quantity ?? 0) * (od?.SellingPrice ?? 0)))
                })
                .ToList();

            return new AdminDashboardStatistics
            {
                TotalOrders = orders.Count,
                OrdersByStatus = orders.CountOrderEachStatus(),
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
                WeeklyStatistics = weeklyStatistics,
                DailyStatistics = dailyStatistics
            };
        }


        public async Task<CountOrders> CountAllOrde()
        {
            var orders = await _unitOfWork.GetRepository<Order>().GetListAsync(
                predicate: o => o.CreatedDate.HasValue,
                include: o => o.Include(o => o.OrderDetails));

            var paidOrCompletedOrders = orders
                .Where(o => o.Status.Equals("PAID", StringComparison.OrdinalIgnoreCase) || o.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase))
                .ToList();

            double totalCost = paidOrCompletedOrders.Sum(o => o.OrderDetails.Sum(od => (od.Quantity ?? 0) * (od?.SellingPrice ?? 0)));
            double totalRevenue = paidOrCompletedOrders.Sum(o => o.FinalAmount ?? 0);
            double totalProfit = totalRevenue - totalCost;


            return new CountOrders
            {
                TolalOrders = orders.Count,
                OrdersByStatus = orders.CountOrderEachStatus(),
                TotalRevenue = totalRevenue,
                TotalProfit = totalProfit,
            };
        }
    }
}
