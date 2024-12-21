using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class AdminOverallStatisticResponse
    {
        public long TotalCustomer {  get; set; }
        public long NumberOfActiveCustomer { get; set; }
        public int NumberOfSalon { get; set; }
        public long NumnberOfBookingCustomer { get; set; }
        public long NumberOfReport {  get; set; }
        public decimal RevenueToday { get; set; }
        public decimal TotalRevenue { get; set; }
        public double ReturnRate { get; set; }
    }
}
