using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class RevenueByHours
    {
        public decimal? TotalRevenue { get; set; }
        public decimal? OutsideRevenue { get; set; }
        public decimal? PlatformRevenue { get; set; }
        public List<HourlyTotalRevenue>? HourlyTotalRevenues { get; set; }
        public List<HourlyOutsideRevenue>? HourlyOutSideRevenues { get; set; }
        public List<HourlyPlatformRevenue>? HourlyPlatformRevenues { get; set; }
    }

    public class HourlyTotalRevenue
    {
        public int? Hour { get; set; } 
        public decimal? Revenue { get; set; } 
    }
    public class HourlyOutsideRevenue
    {
        public int? Hour { get; set; }
        public decimal? Revenue { get; set; }
    }
    public class HourlyPlatformRevenue
    {
        public int? Hour { get; set; }
        public decimal? Revenue { get; set; }
    }
}
