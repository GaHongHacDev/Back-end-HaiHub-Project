using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class StatisticsNumberOfAppointmentOnPlatform
    {
        public long? TotalAppointmenOnPlatform { get; set; }

        public decimal? RateOfOut_SideAppointment { get; set; }
        public decimal? RateOfSuccessedAppointment { get; set; }
        public decimal? RateOfFailAppointment { get; set; }
        public decimal? RateOfCancelAppointment { get; set; }

        public long? NumberOfOut_SideAppointment { get; set; }
        public long? NumberOfSuccessedAppointment { get; set; }
        public long? NumberOfFailAppointment { get; set; }
        public long? NumberOfCancelAppointment { get; set; }

    }

}
