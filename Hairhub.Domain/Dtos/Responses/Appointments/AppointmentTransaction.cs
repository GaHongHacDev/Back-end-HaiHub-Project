using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class AppointmentTransaction
    {
        public List<GetAppointmentTransactionResponse> GetAppointmentTransactionResponse { get; set; }
        public decimal? CurrentComssion { get; set; }
    }
}
