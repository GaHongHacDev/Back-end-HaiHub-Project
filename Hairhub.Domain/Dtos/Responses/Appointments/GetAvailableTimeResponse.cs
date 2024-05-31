using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetAvailableTimeResponse
    {
        List<Decimal> TimeAvailables { get; set; }
    }
}
