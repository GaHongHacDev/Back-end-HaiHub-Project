using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Appointments
{
    public class CancelApointmentRequest
    {
        public string ReasonReport {  get; set; }
        public Guid CustomerId { get; set; }
    }
}
