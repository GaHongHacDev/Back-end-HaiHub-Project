using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Schedule
{
    public class UpdateScheduleEmployeeRequest
    {
        public string DayofWeeks { get; set; }
        
        public TimeOnly StartTime { get; set; }
        
        public TimeOnly EndTime { get; set; }
        public bool IsActive { get; set; }

    }
}
