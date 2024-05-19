using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class UpdateAppointmentRequest
    {
        public DateTime? Date { get; set; }
        public Decimal? TotalPrice { get; set; }
        public bool? IsActive { get; set; }
    }
}
