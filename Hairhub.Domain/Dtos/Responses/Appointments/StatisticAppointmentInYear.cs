using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Appointments
{
    public class StatisticAppointmentInYear
    {
        public string TimeFrame { get; set; } 

        public Dictionary<string, int> TotalAppointments { get; set; } 
        public Dictionary<string, int> OutsideAppointments { get; set; } 
        public Dictionary<string, int> SuccessedAppointments { get; set; }
        public Dictionary<string, int> FailedAppointments { get; set; }
        public Dictionary<string, int> CanceledAppointments { get; set; }

        public StatisticAppointmentInYear()
        {
            OutsideAppointments = new Dictionary<string, int>();
            SuccessedAppointments = new Dictionary<string, int>();
            FailedAppointments = new Dictionary<string, int>();
            CanceledAppointments = new Dictionary<string, int>();
        }
    }
}
