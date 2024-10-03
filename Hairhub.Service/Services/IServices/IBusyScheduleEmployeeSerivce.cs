using Hairhub.Domain.Dtos.Requests.BusySchedule;
using Hairhub.Domain.Dtos.Responses.BusySchedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IBusyScheduleEmployeeSerivce
    {
        Task<string> CreationofaBusySchedule(Guid employeeID, RequestCreationOfBusySchedule request);

        Task<string> UpdateofaBusySchedule(Guid employeeID, RequestCreationOfBusySchedule request);

        Task<bool> DeleteofaBusySchedule(Guid employeeID);

    }
}
