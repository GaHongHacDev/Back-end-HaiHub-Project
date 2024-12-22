using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class RevenueStatistics
    {
        public decimal? TotalRevenue { get; set; } 
        public decimal? OutsideRevenue { get; set; }
        public decimal? PlatformRevenue { get; set; }        
        public int? NumberOfOutsideAppointment { get; set; }
        public int? NumberOfPlatformAppointment { get; set; }
        public int? NumberOfCancelAppointment { get; set; }
        public int? NumberOfFailedAppointment { get; set; } 
        public double? RateOfReturnCustomers { get; set; }
        public decimal? ValueAverageOnProduct { get; set; }

        public decimal? PriceDiscountforCustomers { get; set; }

    }
}
