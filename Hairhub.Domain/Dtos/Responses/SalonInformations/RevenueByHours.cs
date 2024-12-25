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
        public List<HourlyRevenue>? HourlyRevenues { get; set; }
    }

    public class HourlyRevenue
    {
        public int? Hour { get; set; } 
        public decimal? Revenue { get; set; } 
    }
}
