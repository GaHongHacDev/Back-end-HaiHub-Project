﻿using AutoMapper;
using CloudinaryDotNet.Actions;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentDetailService _appointmentDetailService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentDetailService appointmentDetailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentDetailService = appointmentDetailService;
        }
        public async Task<IPaginate<GetAppointmentResponse>> GetAllAppointment(int page, int size)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.Customer),
               page: page,
               size: size
           );

            var scheduleResponses = new Paginate<GetAppointmentResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(appointments.Items),
            };
            return scheduleResponses;
        }

        public async Task<GetAppointmentResponse>? GetAppointmentById(Guid id)
        {
            Appointment appointmentResponse = await _unitOfWork
                .GetRepository<Appointment>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: source => source.Include(a => a.Customer)
                 );
            if (appointmentResponse == null)
                return null;
            return _mapper.Map<GetAppointmentResponse>(appointmentResponse);
        }

        public async Task<CreateAppointmentResponse> CreateAppointment(CreateAppointmentRequest createAccountRequest)
        {
            //Check customer is exist
            var customer = await _unitOfWork.GetRepository<Customer>()
                .SingleOrDefaultAsync(predicate: x => x.Id.Equals(createAccountRequest.CustomerId));
            if (customer == null)
            {
                throw new Exception("CustomerId not found!");
            }
            var appointment = _mapper.Map<Appointment>(createAccountRequest);
            appointment.Id = Guid.NewGuid();
            appointment.IsActive = true;
            appointment.Date = DateTime.Now;
            await _unitOfWork.GetRepository<Appointment>().InsertAsync(appointment);
            await _unitOfWork.CommitAsync();
            if (createAccountRequest.ListAppointmentDetail == null || createAccountRequest.ListAppointmentDetail.Count == 0)
            {
                throw new NotFoundException("AppointmentDetail not found!");
            }
            foreach(var item in createAccountRequest.ListAppointmentDetail)
            {
                try
                {
                    await _appointmentDetailService.CreateAppointmentDetailFromAppointment(appointment.Id, item);
                }
                catch (NotFoundException ex) {
                    throw new NotFoundException(ex.Message);
                }
                catch (Exception ex) 
                {
                    throw new Exception(ex.Message);
                }
            }
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateAppointmentResponse>(appointment);
        }

        public async Task<bool> UpdateAppointmentById(Guid id, UpdateAppointmentRequest updateAppointmentRequest)
        {
            var appoinment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinment == null)
            {
                throw new NotFoundException("Appoint not found!");
            }
            appoinment = _mapper.Map<Appointment>(updateAppointmentRequest);
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<bool> DeleteAppoinmentById(Guid id)
        {
            var appoinment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinment == null)
            {
                throw new NotFoundException("Appoint not found!");
            }
            appoinment.IsActive = false;
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<bool> ActiveAppointment(Guid id)
        {
            var appoinment = await _unitOfWork.GetRepository<Appointment>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (appoinment == null)
            {
                throw new NotFoundException("Appoint not found!");
            }
            appoinment.IsActive = true;
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<GetAvailableTimeResponse> GetAvailableTime(GetAvailableTimeRequest request)
        {
            GetAvailableTimeResponse result = new GetAvailableTimeResponse();
            if (!request.IsAnyOne)
            {
                //Xử lý khi có Employee cố định
                var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(
                                                predicate: x => x.SalonInformationId == request.SalonId && x.Id == request.SalonEmployeeId);
                if (employee == null)
                {
                    throw new NotFoundException("Employee not found in salon, baber shop!");
                }
                var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                                    predicate: x=>x.EmployeeId == employee.Id 
                                                                && x.Date.Equals(request.Day.DayOfWeek.ToString()));
                var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute/60;
                var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute/60;
                List<decimal> TimeSlot = generateTimeSlot(startSchedule, endSchedule, (decimal)0.25);
  
                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                                    predicate: x=>x.SalonEmployeeId == request.SalonEmployeeId 
                                                    && x.StartTime.Value.Date == request.Day.Date
                                                    && x.EndTime.Value.Date == request.Day.Date);
                foreach (var item in appointmentDetails)
                {
                    decimal start = (decimal)item.StartTime.Value.TimeOfDay.TotalHours;
                    decimal end = (decimal)item.EndTime.Value.TimeOfDay.TotalHours;

                    TimeSlot.RemoveAll(slot => slot >= start && slot < end);
                }
                result.TimeAvailables = TimeSlot;
            }
            else
            {
                //Xủ lý khi chọn employee nào cũng được
                var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(
                                                predicate: x => x.SalonInformationId == request.SalonId);
                if (employees == null)
                {
                    throw new NotFoundException("Employee not found in salon, baber shop!");
                }
                List<decimal> TimeSlot = new List<decimal>();

                foreach (var employee in employees)
                {   // Get schedule by id
                    var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                    predicate: x => x.EmployeeId == employee.Id
                                     && x.Date.Equals(request.Day.DayOfWeek.ToString()));
                    //Get Time work of employee
                    var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                    var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60;
                    //Define List time work
                    List<decimal> TimeSlotEmployee = generateTimeSlot(startSchedule, endSchedule, (decimal)0.25);
                    //Get appointment detail => Check available time
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                                        predicate: x => x.SalonEmployeeId == request.SalonEmployeeId
                                                        && x.StartTime.Value.Date == request.Day.Date
                                                        && x.EndTime.Value.Date == request.Day.Date);
                    foreach (var item in appointmentDetails)
                    {
                        decimal start = (decimal)item.StartTime.Value.TimeOfDay.TotalHours;
                        decimal end = (decimal)item.EndTime.Value.TimeOfDay.TotalHours;

                        TimeSlotEmployee.RemoveAll(slot => slot >= start && slot < end);
                    }
                    TimeSlot = TimeSlot.Union(TimeSlotEmployee).ToList();
                }
                result.TimeAvailables = TimeSlot;
            }
            return result;
        }

        private List<decimal> generateTimeSlot(decimal begin, decimal end, decimal step)
        {
            int count = (int)((end - begin) / step) + 1;
            List<decimal> array = new List<decimal>();
            for (int i = 0; i < count; i++)
            {
                array.Add(begin + i * step);
            }
            return array;
        }

        public async Task<IPaginate<GetAppointmentByAccountIdResponse>> GetAppointmentByAccountId(Guid AccountId, int page, int size)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x=>x.AccountId==AccountId);
            if (customer == null)
            {
                throw new NotFoundException($"Not found customer with id {AccountId}");
            }
            var appointments = await _unitOfWork.GetRepository<Appointment>()
           .GetPagingListAsync(
               page: page,
               size: size
           );

            var appointmentResponse = new Paginate<GetAppointmentByAccountIdResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentByAccountIdResponse>>(appointments.Items),
            };
            foreach ( var item in appointmentResponse.Items)
            {
                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                    .GetListAsync(predicate: x=>x.AppointmentId == item.Id);
                item.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
            }
            return appointmentResponse;
        }
    }
}
