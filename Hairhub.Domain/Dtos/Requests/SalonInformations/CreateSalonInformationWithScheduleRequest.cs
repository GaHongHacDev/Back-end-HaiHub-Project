using Hairhub.Domain.Dtos.Requests.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.SalonInformations
{
    public class CreateSalonInformationWithScheduleRequest : CreateSalonInformationRequest
    {
        public List<CreateScheduleRequest> Schedules { get; set; }
    }
}
