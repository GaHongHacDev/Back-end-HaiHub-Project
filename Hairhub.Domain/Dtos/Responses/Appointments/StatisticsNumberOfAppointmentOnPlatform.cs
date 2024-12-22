using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class StatisticsNumberOfAppointmentOnPlatform
    {
        public int? TotalAppointmenOnPlatform { get; set; }

        public decimal? RateOfOut_SideAppointment { get; set; }
        public decimal? RateOfSuccessedAppointment { get; set; }
        public decimal? RateOfFailAppointment { get; set; }
        public decimal? RateOfCancelAppointment { get; set; }

    }

}
