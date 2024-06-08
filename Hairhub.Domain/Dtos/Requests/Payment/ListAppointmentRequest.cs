using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Payment
{
    public class ListAppointmentRequest
    {       
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public string CustomerName { get; set; }
    }
}
