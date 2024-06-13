﻿using AutoMapper;
using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Requests.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.SalonEmployees;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Hairhub.Service.Services.Services
{
    public class SalonEmployeeService : ISalonEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IScheduleService _scheduleService;

        public SalonEmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService, IScheduleService scheduleService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
            _scheduleService = scheduleService;
        }

        public async Task<bool> ActiveSalonEmployee(Guid id)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonEmployee == null)
            {
                throw new NotFoundException("SalonEmployee not found!");
            }
            salonEmployee.IsActive = true;
            _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(salonEmployee);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<bool> CreateSalonEmployee(CreateSalonEmployeeRequest request)
        {
            //check exist salon
            var existSalon = await _unitOfWork.GetRepository<SalonInformation>()
                                        .SingleOrDefaultAsync(predicate: x=>x.Id == request.SalonInformationId);
            if (existSalon==null)
            {
                throw new NotFoundException($"Not found salon with id {request.SalonInformationId}");
            }
            //create employee
            foreach(var item in request.SalonEmployees)
            {
                var employee = _mapper.Map<SalonEmployee>(item);
                employee.Id = Guid.NewGuid();
                var url = await _mediaService.UploadAnImage(item.ImgEmployee, MediaPath.EMPLOYEE, employee.Id.ToString());
                employee.Img = url;
                employee.SalonInformationId = request.SalonInformationId;
                await _unitOfWork.GetRepository<SalonEmployee>().InsertAsync(employee);
                if (item.ScheduleEmployees==null || item.ScheduleEmployees.Count==0)
                {
                    throw new NotFoundException("Không tìm thấy lịch làm việc của nhân viên");
                }
                //create schedule
                foreach (var itemSchedule in item.ScheduleEmployees)
                {
                    var scheduleEmployee = new CreateScheduleRequest() 
                                                {
                                                    EmployeeId = employee.Id, 
                                                    DayOfWeek=itemSchedule.Date, 
                                                    EndTime= itemSchedule.EndTime, 
                                                    StartTime= itemSchedule.StartTime
                                                };
                    _scheduleService.CreateScheduleEmployee(scheduleEmployee);
                }
                //create Service Employee
                foreach(var itemServiceHair in item.ServiceHairId)
                {
                    ServiceEmployee srvEmployee = new ServiceEmployee();
                    srvEmployee.Id = Guid.NewGuid();
                    srvEmployee.ServiceHairId = itemServiceHair;
                    srvEmployee.SalonEmployeeId = employee.Id;
                    await _unitOfWork.GetRepository<ServiceEmployee>().InsertAsync(srvEmployee);
                }
            }
            existSalon.IsActive = true;
            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(existSalon);
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            return isInsert;
        }

        public async Task<bool> DeleteSalonEmployeeById(Guid id)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonEmployee == null)
            {
                throw new NotFoundException("SalonEmployee not found!");
            }
            _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(salonEmployee);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<IPaginate<GetSalonEmployeeResponse>> GetAllSalonEmployee(int page, int size)
        {
            var salonEmployees = await _unitOfWork.GetRepository<SalonEmployee>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.SalonInformation),
               page: page,
               size: size
           );

            var salonEmployeeResponses = new Paginate<GetSalonEmployeeResponse>()
            {
                Page = salonEmployees.Page,
                Size = salonEmployees.Size,
                Total = salonEmployees.Total,
                TotalPages = salonEmployees.TotalPages,
                Items = _mapper.Map<IList<GetSalonEmployeeResponse>>(salonEmployees.Items),
            };
            return salonEmployeeResponses;
        }

        public async Task<GetSalonEmployeeResponse>? GetSalonEmployeeById(Guid id)
        {
            SalonEmployee salonEmployeeResponse = await _unitOfWork
                .GetRepository<SalonEmployee>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: x => x.Include(se => se.ServiceEmployees)
                       .ThenInclude(se => se.ServiceHair).Include(se => se.Schedules)
                 );
            if (salonEmployeeResponse == null)
                return null;
            return _mapper.Map<GetSalonEmployeeResponse>(salonEmployeeResponse);
        }

        public async Task<IPaginate<GetSalonEmployeeResponse>> GetSalonEmployeeBySalonInformationId(Guid SalonInformationId, int page, int size)
        {
            var salonEmployees = await _unitOfWork.GetRepository<SalonEmployee>()
                                                  .GetPagingListAsync(
                                                      predicate: x => x.SalonInformationId.Equals(SalonInformationId),
                                                      include: query => query.Include(s => s.Schedules)
                                                                             .Include(s => s.ServiceEmployees)
                                                                             .ThenInclude(se => se.ServiceHair),
                                                      page: page,
                                                      size: size
                                                  );
            if (salonEmployees == null)
                return null;
            var employeeResponse = _mapper.Map<IList<GetSalonEmployeeResponse>>(salonEmployees.Items);
            foreach (var salonEmployee in employeeResponse)
            {
                var schedules = await _unitOfWork.GetRepository<Schedule>()
                                                 .GetListAsync(predicate: x => x.EmployeeId == salonEmployee.Id);
                salonEmployee.Schedules = _mapper.Map<List<ScheduleEmployeeResponse>>(schedules);
            }
            var salonEmployeeResponses = new Paginate<GetSalonEmployeeResponse>()
            {
                Page = salonEmployees.Page,
                Size = salonEmployees.Size,
                Total = salonEmployees.Total,
                TotalPages = salonEmployees.TotalPages,
                Items = employeeResponse,
            };
            return salonEmployeeResponses;
        }

        public async Task<bool> UpdateSalonEmployeeById(Guid id, UpdateSalonEmployeeRequest updateSalonEmployeeRequest)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonEmployee == null)
            {
                throw new NotFoundException("SalonEmployee not found!");
            }
            salonEmployee = _mapper.Map<SalonEmployee>(updateSalonEmployeeRequest);
            _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(salonEmployee);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
    }
}
