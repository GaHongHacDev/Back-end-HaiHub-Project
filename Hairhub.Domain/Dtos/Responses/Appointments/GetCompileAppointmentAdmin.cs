using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class GetCompileAppointmentAdmin
    {
        public List<CompileAppointmentByDayOfWeek> CompileAppointmentByDayOfWeek { get; set; }
        public double SuccessedAppointmentPercent { get; set; }
        public double CancelAppointmentPercent { get; set; }
        public double FailedAppointmentPercent { get; set; }
    }

    public class CompileAppointmentByDayOfWeek
    {
        public string DayOfWeek { get; set; }
        public long NumberOfSuccessedAppointment { get; set; }
    }
}
