using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.SalonInformations
{
    public class CompileAppointmentSalonByYearResponse
    {
       public decimal CancelAppointmentPercent { get; set; }
       public decimal FailedAppointmentPercent { get; set; }
       public decimal OutSideAppointmentPercent { get; set; }
       public decimal InSideAppointmentPercent { get; set; }
       public List<AppointmentStatistic> revenuewStatistics { get; set; } = new List<AppointmentStatistic>();
    }

    public class AppointmentStatistic
    {
        public string Month { get; set; }
        public decimal OutSideAppointment { get; set; }
        public decimal InSideAppointment { get; set; }
        public decimal FailedAppointment { get; set; }
        public decimal CancelAppointment { get; set; }
        public decimal TotalAppointment { get; set; }
    }
}
