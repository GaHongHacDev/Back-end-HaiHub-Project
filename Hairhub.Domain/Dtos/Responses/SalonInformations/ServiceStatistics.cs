using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class ServiceStatistics
    {
        public string? ServiceName { get; set; }

        public int? NumberOfUses { get; set; }

        public int? NumberOfCustomers { get; set; }

        public decimal? RevenueFromService { get; set; }
    }
}
