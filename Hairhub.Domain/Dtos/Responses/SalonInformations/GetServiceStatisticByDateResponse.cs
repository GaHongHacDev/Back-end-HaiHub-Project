using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class GetServiceStatisticByDateResponse
    {
        public Guid Id { get; set; }
        public string ServiceName { get; set; }
        public int NumberOfUses { get; set; }
    }
}
