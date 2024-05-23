using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Config
{
    public class CreateConfigResponse
    {
        public decimal? CommissionRate { get; set; }
        public decimal? MaintenanceFee { get; set; }
        public DateTime? DateCreate { get; set; }
        public bool? IsActive { get; set; }
    }
}
