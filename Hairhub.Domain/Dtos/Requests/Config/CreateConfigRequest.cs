using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Config
{
    public class CreateConfigRequest
    {
        public string PakageName { get; set; }
        public string Description { get; set; }
        public decimal PakageFee { get; set; }
    }
}
