using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IScheduleService
    {
        Task<IPaginate<GetScheduleResponse>> GetSchedules(int page, int size);
        Task<GetScheduleResponse> GetScheduleById(Guid id);
        Task<bool> CreateSchedule(CreateScheduleRequest request);
        Task<bool> UpdateSchedule(Guid id, UpdateScheduleRequest request);
        Task<bool> DeleteSchedule(Guid id);
        public Task<bool> CreateScheduleEmployee(CreateScheduleRequest request);
    }
}
