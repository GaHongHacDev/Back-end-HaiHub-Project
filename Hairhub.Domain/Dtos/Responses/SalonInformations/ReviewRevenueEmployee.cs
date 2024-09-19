using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class ReviewRevenueEmployee
    {
        public string FullName { get; set; }
        public string? Gender { get; set; }
        public string Phone { get; set; }
        public string Img { get; set; }
        public string Email { get; set; }
        public int totalSuccessedAppointment {  get; set; } = 0;
        public int totalFailedAppointment { get; set; } = 0;
        public int totalCanceledAppointment { get; set; } = 0;
        public decimal totalRevuenueEmployee { get; set; } = 0;
    }
}
