using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.BusySchedule;
using Hairhub.Domain.Entitities;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Dtos.Responses.BusySchedule;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;

namespace Hairhub.Service.Services.Services
{
    public class BusyScheduleEmployeeSerivce : IBusyScheduleEmployeeSerivce
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentDetailService _appointmentDetailService;

        public BusyScheduleEmployeeSerivce(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentDetailService appointmentDetailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentDetailService = appointmentDetailService;
        }

        public async Task<string> CreationofaBusySchedule(Guid employeeID, RequestCreationOfBusySchedule request)
        {
            
            var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: p => p.Id == employeeID);
            if (employee == null)
            {
                throw new Exception("Nhân viên này không tồn tại");
            }

            
            CreationOfBusyScheduleResponse response = null;
            string message = string.Empty;

            
            if (request != null)
            {
                
                var appointments = await _unitOfWork.GetRepository<Appointment>()
                    .GetListAsync(
                        predicate: p => p.AppointmentDetails
                            .Any(s => s.SalonEmployeeId == employee.Id && s.StartTime > request.StartDate && s.StartTime <= request.EndDate && s.Status == AppointmentStatus.Booking),
                        include: s => s.Include(s => s.AppointmentDetails).Include(s => s.Customer),
                        orderBy: s => s.OrderBy(d => d.StartDate)
                    );

                
                if (appointments != null && appointments.Count >= 1)
                {
                    var listapp = new List<CreationOfBusyScheduleResponse>();

                    foreach (var appointment in appointments)
                    {
                        
                        var customerName = appointment.Customer != null ? appointment.Customer.FullName : "Unknown Customer";                    
                        var appointmentDetails = (await _appointmentDetailService.GetAppointmentDetailByAppointmentId(appointment.Id) as List<GetAppointmentDetailResponse>) ?? new List<GetAppointmentDetailResponse>();
                        response = new CreationOfBusyScheduleResponse
                        {
                            CustomerName = customerName,
                            StartTime = appointment.StartDate.Date,
                            AppointmentDetails = appointmentDetails
                        };

                        listapp.Add(response);
                    }
                    var appointmentDetailsMessage = string.Join("\n", listapp.Select(app =>
                        $"Customer Name: {app.CustomerName}, Appointment Start Time: {app.StartTime}, Appointment Details: " +
                        string.Join(", ", app.AppointmentDetails.Select(ad =>
                            $"[Start: {ad.StartTime}, End: {ad.EndTime}, Service: {ad.ServiceName}]"))
                    ));
                    message = $"Bạn không thể thêm lịch bận vì còn lịch hẹn khác:\n{appointmentDetailsMessage}";
                    return message;
                }
                var busySchedule = new BusyScheduleEmployee
                {
                    Id = Guid.NewGuid(),
                    EmployeeId = employee.Id,
                    StartTime = request.StartDate ?? DateTime.MinValue, 
                    EndTime = request.EndDate ?? DateTime.MinValue, 
                    Note = request.Note,
                    Status = BusyScheduleStatus.Successed,
                };
                await _unitOfWork.GetRepository<BusyScheduleEmployee>().InsertAsync(busySchedule);
                await _unitOfWork.CommitAsync();
                message = "Bạn đã thêm lịch bận thành công";
                return message;
            }
            return null;
        }

        public async Task<bool> DeleteofaBusySchedule(Guid employeeID)
        {
            var employee = await _unitOfWork.GetRepository<BusyScheduleEmployee>()
                                .SingleOrDefaultAsync(predicate: p => p.EmployeeId == employeeID && p.Status == BusyScheduleStatus.Successed);
            if (employee == null)
            {
                throw new Exception("Nhân viên này không không có lịch bận");
            }
            employee.Status = BusyScheduleStatus.Fail;
            employee.Id = employeeID;
             _unitOfWork.GetRepository<BusyScheduleEmployee>().UpdateAsync(employee);
            bool isDeleted = await _unitOfWork.CommitAsync() > 0;
            return isDeleted;

        }

        public async Task<string> UpdateofaBusySchedule(Guid employeeID, RequestCreationOfBusySchedule request)
        {
            var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: p => p.Id == employeeID);
            if (employee == null)
            {
                throw new Exception("Nhân viên này không tồn tại");
            }
            var busySchedule = await _unitOfWork.GetRepository<BusyScheduleEmployee>().SingleOrDefaultAsync(predicate: p => p.EmployeeId == employee.Id);

            CreationOfBusyScheduleResponse response = null;
            string message = string.Empty;


            if (request != null)
            {

                var appointments = await _unitOfWork.GetRepository<Appointment>()
                    .GetListAsync(
                        predicate: p => p.AppointmentDetails
                            .Any(s => s.SalonEmployeeId == employee.Id && s.StartTime > request.StartDate && s.StartTime <= request.EndDate && s.Status == AppointmentStatus.Booking),
                        include: s => s.Include(s => s.AppointmentDetails).Include(s => s.Customer),
                        orderBy: s => s.OrderBy(d => d.StartDate)
                    );


                if (appointments != null && appointments.Count >= 1)
                {
                    var listapp = new List<CreationOfBusyScheduleResponse>();

                    foreach (var appointment in appointments)
                    {

                        var customerName = appointment.Customer != null ? appointment.Customer.FullName : "Unknown Customer";
                        var appointmentDetails = (await _appointmentDetailService.GetAppointmentDetailByAppointmentId(appointment.Id) as List<GetAppointmentDetailResponse>) ?? new List<GetAppointmentDetailResponse>();
                        response = new CreationOfBusyScheduleResponse
                        {
                            CustomerName = customerName,
                            StartTime = appointment.StartDate.Date,
                            AppointmentDetails = appointmentDetails
                        };

                        listapp.Add(response);
                    }
                    var appointmentDetailsMessage = string.Join("\n", listapp.Select(app =>
                        $"Customer Name: {app.CustomerName}, Appointment Start Time: {app.StartTime}, Appointment Details: " +
                        string.Join(", ", app.AppointmentDetails.Select(ad =>
                            $"[Start: {ad.StartTime}, End: {ad.EndTime}, Service: {ad.ServiceName}]"))
                    ));
                    message = $"Bạn không thể thêm lịch bận vì còn lịch hẹn khác:\n{appointmentDetailsMessage}";
                    return message;
                }
                busySchedule.StartTime = (DateTime)request.StartDate;
                busySchedule.EndTime = (DateTime)request.EndDate;
                busySchedule.Note = request.Note; 
                 _unitOfWork.GetRepository<BusyScheduleEmployee>().UpdateAsync(busySchedule);
                await _unitOfWork.CommitAsync();
                message = "Bạn đã cập nhật bận thành công";
                return message;
            }
            return null;
        }
    }
}
