using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetAppointmentTodayAdminResponse
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string SalonName { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal CommissionRevenue { get; set; }
    }
}
