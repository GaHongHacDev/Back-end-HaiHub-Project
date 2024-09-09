using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class AppointmentTransaction
    {
        public Guid Id { get; set; }
        public DateTime StartDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }

        public decimal? CommissionRate { get; set; }
    }
}
