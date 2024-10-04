using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.BusySchedule
{
    public class CreationOfBusyScheduleResponse
    {
        public List<GetAppointmentDetailResponse> AppointmentDetails { get; set; }  
        public string CustomerName { get; set; }    
        public DateTime StartTime { get; set; }     
    }
}
