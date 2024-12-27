using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Config
{
    public class CreateSubscriptionConfigRequest
    {
        public string PakageName { get; set; }
        public string Description { get; set; }
        public decimal? PakageFee { get; set; }
        public int? NumberOfDay { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
    }
}
