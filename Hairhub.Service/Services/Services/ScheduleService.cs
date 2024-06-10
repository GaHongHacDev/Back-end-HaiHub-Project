using AutoMapper;
using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ScheduleService(IUnitOfWork _unitOfWork, IMapper _mapper)
        {
            this._unitOfWork = _unitOfWork;
            this._mapper = _mapper;
        }

        public async Task<IPaginate<GetScheduleResponse>> GetSchedules(int page, int size)
        {
            var schedules = await _unitOfWork.GetRepository<Schedule>()
            .GetPagingListAsync(
                predicate: x => x.IsActive == true,
                include: query => query.Include(s => s.SalonEmployee),
                page: page,
                size: size
            );

            var scheduleResponses = new Paginate<GetScheduleResponse>()
            {
                Page = schedules.Page,
                Size = schedules.Size,
                Total = schedules.Total,
                TotalPages = schedules.TotalPages,
                Items = _mapper.Map<IList<GetScheduleResponse>>(schedules.Items),
            };

            return scheduleResponses;
        }

        public async Task<List<GetScheduleResponse>> GetSalonSchedules(Guid salonId)
        {
            var salonInfo = _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x=>x.Id == salonId);
            if(salonInfo == null)
            {
                throw new NotFoundException($"Cannot find schedule with salon id {salonId}");
            }
            var schedules = await _unitOfWork.GetRepository<Schedule>()
            .GetListAsync(
                include: query => query.Include(s => s.SalonInformation)
            );
            return _mapper.Map<List<GetScheduleResponse>>(schedules);
        }

        public async Task<GetScheduleResponse> GetScheduleById(Guid id)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>()
               .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id),
               include: x => x.Include(s => s.SalonEmployee));

            var scheduleResponse = _mapper.Map<GetScheduleResponse>(schedule);

            return scheduleResponse;
        }

        public async Task<bool> CreateSchedule(CreateScheduleRequest request)
        {
            Schedule newSchedule = new Schedule()
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = true,
            };
            await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> CreateScheduleEmployee(CreateScheduleRequest request)
        {
            Schedule newSchedule = new Schedule()
            {
                Id = Guid.NewGuid(),
                EmployeeId = request.EmployeeId,
                DayOfWeek = request.DayOfWeek,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                IsActive = true,
            };
            await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);
            return true;
        }

        public async Task<bool> UpdateSchedule(Guid id, UpdateScheduleRequest request)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (schedule == null) throw new Exception("Schedule is not exist!!!");

            schedule.DayOfWeek = request.DayOfWeek;
            schedule.StartTime = request.StartTime;
            schedule.EndTime = request.EndTime;

            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }

        public async Task<bool> DeleteSchedule(Guid id)
        {
            var schedule = await _unitOfWork.GetRepository<Schedule>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(id));

            if (schedule == null) throw new Exception("Schedule is not exist!!!");

            schedule.IsActive = false;
            _unitOfWork.GetRepository<Schedule>().UpdateAsync(schedule);
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}
