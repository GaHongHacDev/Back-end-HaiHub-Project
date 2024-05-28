using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Config
{
    public class GetConfigResponse
    {
        public GetConfigResponse(decimal? commissionrate, decimal? maintenancefee, DateTime? datecreate, bool? isactive) { 
            CommissionRate = commissionrate;
            MaintenanceFee = maintenancefee;
            DateCreate = datecreate;
            IsActive = isactive;
        
        
        }

        public decimal? CommissionRate { get; set; }
        public decimal? MaintenanceFee { get; set; }
        public DateTime? DateCreate { get; set; }
        public bool? IsActive { get; set; }
    }
}
