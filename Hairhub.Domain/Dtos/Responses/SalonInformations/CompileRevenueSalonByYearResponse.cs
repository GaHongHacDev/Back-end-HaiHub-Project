using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class CompileRevenueSalonByYearResponse
    {
        public decimal OutSideRevenuePercent { get; set; }
        public decimal InSideRevenuePercent { get; set; }
        public List<RevenueStatistic> revenuewStatistics { get; set; } = new List<RevenueStatistic>();
    }

    public class RevenueStatistic
    {
        public string Month { get; set; }
        public decimal OutSideRevenue {  get; set; }
        public decimal InSideRevenue {  get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
