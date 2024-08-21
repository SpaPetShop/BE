using Meta.BusinessTier.Enums.Status;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using Meta.BusinessTier.Enums.EnumStatus;
using Meta.BusinessTier.Utils;
using Meta.DataTier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Meta.BusinessTier.Extensions
{
    public static class DataExtensions
    {
        public static Dictionary<RoleEnum, int> CountAccountEachRoleEnum(this ICollection<Account> accounts)
        {
            var statusCount = new Dictionary<RoleEnum, int>();

            foreach(RoleEnum status in Enum.GetValues(typeof(RoleEnum)))
            {
                string statusDes = status.GetDescriptionFromEnum();
                int count = accounts.Count(item => item.Role.Equals(statusDes));
                statusCount.Add(status, count);
            }
            return statusCount;
        }
        public static Dictionary<UserStatus, int> CountAccountEachStatus(this ICollection<Account> accounts)
        {
            var statusCount = new Dictionary<UserStatus, int>();

            foreach (UserStatus status in Enum.GetValues(typeof(UserStatus)))
            {
                string statusDes = status.GetDescriptionFromEnum();
                int count = accounts.Count(item => item.Status.Equals(statusDes));
                statusCount.Add(status, count);
            }
            return statusCount;
        }
        public static Dictionary<OrderStatus, int> CountOrderEachStatus(this ICollection<Order> orders)
        {
            var statusCount = new Dictionary<OrderStatus, int>();

            foreach (OrderStatus status in Enum.GetValues(typeof(OrderStatus)))
            {
                string statusDes = status.GetDescriptionFromEnum();
                int count = orders.Count(item => item.Status.Equals(statusDes));
                statusCount.Add(status, count);
            }
            return statusCount;
        }
        public static Dictionary<TaskManagerStatus, int> CountTaskEachStatus(this ICollection<TaskManager> task)
        {
            var statusCount = new Dictionary<TaskManagerStatus, int>();

            foreach (TaskManagerStatus status in Enum.GetValues(typeof(TaskManagerStatus)))
            {
                string statusDes = status.GetDescriptionFromEnum();
                int count = task.Count(item => item.Status.Equals(statusDes));
                statusCount.Add(status, count);
            }
            return statusCount;
        }

    }
}
