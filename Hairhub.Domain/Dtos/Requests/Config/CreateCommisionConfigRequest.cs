using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Config
{
    public class CreateCommisionConfigRequest
    {
        public string PakageName { get; set; }
        public string Description { get; set; }
        public decimal? CommissionRate { get; set; }
        public bool IsActive { get; set; }
        public string Type { get; set; }
    }
}
