using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetAppointmentTransactionResponse
    {
        public List<AppointmentTransaction> AppointmentTransactions { get; set; }
        public decimal? CurrentComssion { get; set; } = 0;
        public decimal? TotalComssion { get; set; } = 0;
    }
}
