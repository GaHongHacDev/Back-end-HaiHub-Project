using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Config
{
    public class UpdateConfigRequest
    {
        public string PakageName { get; set; }
        public string Description { get; set; }
        public decimal? PakageFee { get; set; }
        public DateTime DateCreate { get; set; }
        public decimal? CommissionRate { get; set; }
        public int? NumberOfDay { get; set; }
        public bool IsActive { get; set; }
    }
}
