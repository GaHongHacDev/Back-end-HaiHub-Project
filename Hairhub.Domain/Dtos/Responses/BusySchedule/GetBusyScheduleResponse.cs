using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.BusySchedule
{
    public class GetBusyScheduleResponse
    {
        public Guid Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Title { get; set; }
        public bool IsBusySchedule { get; set; }
        public Guid? IdAppointment { get; set; }
    }
}
