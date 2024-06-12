using AutoMapper;
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
using Org.BouncyCastle.Asn1.Ocsp;
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

        #region GET

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

        public async Task<IPaginate<GetAppointmentResponse>> GetHistoryAppointmentByCustomerId(int page, int size, Guid CustomerId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                               .GetPagingListAsync(
                                                   predicate: x => x.CustomerId == CustomerId && (x.Status.Equals(AppointmentStatus.Successed)
                                                              || x.Status.Equals(AppointmentStatus.CancelByCustomer)
                                                              || x.Status.Equals(AppointmentStatus.CancelBySalon)),
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
            if (appointments != null)
            {
                foreach (var item in appointments.Items)
                {
                    var apoointmentDetails = await _appointmentDetailService.GetAppointmentDetailByAppointmentId(item.Id);
                    if (apoointmentDetails != null)
                    {
                        item.AppointmentDetails = (ICollection<AppointmentDetail>)apoointmentDetails;
                    }
                }
            }
            return scheduleResponses;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetBookingAppointment(int page, int size, Guid CustomerId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                               .GetPagingListAsync(
                                                   predicate: x => x.CustomerId == CustomerId && x.Status.Equals(AppointmentStatus.Booking),
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
            if (appointments != null)
            {
                foreach (var item in appointments.Items)
                {
                    var apoointmentDetails = await _appointmentDetailService.GetAppointmentDetailByAppointmentId(item.Id);
                    if (apoointmentDetails != null)
                    {
                        item.AppointmentDetails = (ICollection<AppointmentDetail>)apoointmentDetails;
                    }
                }
            }
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

        public async Task<IPaginate<GetAppointmentByAccountIdResponse>> GetAppointmentByAccountId(Guid AccountId, int page, int size)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.AccountId == AccountId);
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
            foreach (var item in appointmentResponse.Items)
            {
                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                    .GetListAsync(predicate: x => x.AppointmentId == item.Id);
                item.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
            }
            return appointmentResponse;
        }

        #endregion

        #region Booking

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
                    throw new NotFoundException("Không tìm thấy nhân viên của salon, barber shop");
                }
                var scheduleEmp = await _unitOfWork.GetRepository<Schedule>()
                                                   .SingleOrDefaultAsync
                                                    (
                                                        predicate: x => x.EmployeeId == employee.Id && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString())
                                                    );
                var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60; // Thời gian làm việc của employee
                var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60;
                List<decimal> TimeSlot = GenerateTimeSlot(startSchedule, endSchedule, (decimal)0.25);

                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                          .GetListAsync
                                                           (
                                                                predicate: x => x.SalonEmployeeId == request.SalonEmployeeId
                                                                                && x.StartTime.Date == request.Day.Date
                                                                                && x.EndTime.Date == request.Day.Date
                                                           );
                foreach (var item in appointmentDetails)
                {
                    decimal start = (decimal)item.StartTime.TimeOfDay.TotalHours;
                    decimal end = (decimal)item.EndTime.TimeOfDay.TotalHours;
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
                    throw new NotFoundException("Không tìm thấy nhân viên của salon, barber shop");
                }
                List<decimal> TimeSlot = new List<decimal>();

                foreach (var employee in employees)
                {   // Get schedule by id
                    var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                    predicate: x => x.EmployeeId == employee.Id
                                     && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()));
                    //Get Time work of employee
                    var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                    var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60;
                    //Define List time work
                    List<decimal> TimeSlotEmployee = GenerateTimeSlot(startSchedule, endSchedule, (decimal)0.25);
                    //Get appointment detail => Check available time
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                                        predicate: x => x.SalonEmployeeId == request.SalonEmployeeId
                                                        && x.StartTime.Date == request.Day.Date
                                                        && x.EndTime.Date == request.Day.Date
                                                        && x.Status.Equals(AppointmentStatus.Booking));
                    foreach (var item in appointmentDetails)
                    {
                        decimal start = (decimal)item.StartTime.TimeOfDay.TotalHours;
                        decimal end = (decimal)item.EndTime.TimeOfDay.TotalHours;
                        TimeSlotEmployee.RemoveAll(slot => slot >= start && slot < end);
                    }
                    TimeSlot = TimeSlot.Union(TimeSlotEmployee).ToList();
                }
                result.TimeAvailables = TimeSlot;
            }
            return result;
        }

        public async Task<BookAppointmentResponse> BookAppointment(BookAppointmentRequest request)
        {
            BookAppointmentResponse bookingResponse = new BookAppointmentResponse();
            var firstBookingDetail = request.BookingDetail[0];
            DateTime DateBooking = new DateTime(request.Day.Year, request.Day.Month, request.Day.Day, (int)request.AvailableSlot, (int)((request.AvailableSlot - (int)request.AvailableSlot) * 60), 0);
            //Get Serrvice Hair
            var serviceHair = await _unitOfWork.GetRepository<ServiceHair>()
                                     .SingleOrDefaultAsync
                                      (
                                        predicate: x => x.Id == firstBookingDetail.ServiceHairId && x.SalonInformationId == request.SalonId
                                      );
            if (serviceHair == null)
            {
                throw new NotFoundException($"Không tìm thấy dịch vụ với id {firstBookingDetail.ServiceHairId} của salon id {request.SalonId} ");
            }
            //Get thời gian kết thúc sau khi thực hiện srv hair
            Decimal endTimeProcess = request.AvailableSlot + serviceHair.Time;
            //Get Schedule of salon in Day Of Week
            var scheduleSolon = await _unitOfWork.GetRepository<Schedule>()
                                             .SingleOrDefaultAsync
                                              (
                                                predicate: x => x.SalonId == request.SalonId && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()) && x.IsActive == true
                                              );
            if (scheduleSolon == null)
            {
                throw new NotFoundException($"Salon, barber shop không hoạt động vào {(int)request.AvailableSlot}:{(int)((request.AvailableSlot - (int)request.AvailableSlot) * 60)}");
            }
            //check end time of schedule có đủ thời gian thực hiện srv hair không 
            Decimal endTimeSalon = scheduleSolon.EndTime.Hour + (scheduleSolon.EndTime.Minute) / 60m;
            if (endTimeSalon < endTimeProcess)
            {
                throw new Exception("Thời gian thực hiện dịch vụ quá thời gian làm việc của salon, barber shop");
            }
            //Get List Employee can implement this service hair
            List<EmployeeAvailable> listEmp = new List<EmployeeAvailable>();
            if (request.BookingDetail[0].IsAnyOne)
            {
                //Xủ lý khi chọn employee nào cũng được => IsAnyOne = true
                var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(
                                                predicate: x => x.SalonInformationId == request.SalonId);
                if (employees == null)
                {
                    throw new NotFoundException("Không tìm thấy nhân viên của salon, barber shop");
                }
                foreach (var employee in employees)
                {   // Get schedule by id
                    var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                    predicate: x => x.EmployeeId == employee.Id
                                     && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()));
                    //Get Time work of employee
                    var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                    var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60; //8.5 => 8h30
                    //Get appointment detail => Check available time
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                                        predicate: x => x.SalonEmployeeId == employee.Id
                                                        && x.StartTime.Date == request.Day.Date && x.StartTime <= DateBooking
                                                        && x.EndTime.Date == request.Day.Date && x.EndTime > DateBooking
                                                        && x.Status.Equals(AppointmentStatus.Booking));
                    if (appointmentDetails == null)
                    {
                        listEmp.Add(new EmployeeAvailable() { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                    }
                }
            }
            else
            {
                //Xủ lý khi chọn employee cố định => IsAnyOne = false
                var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(
                                predicate: x => x.Id == firstBookingDetail.SalonEmployeeId);
                if (employee == null)
                {
                    throw new NotFoundException($"Không tìm thấy employee với id {firstBookingDetail.SalonEmployeeId}");
                }
                var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                predicate: x => x.EmployeeId == employee.Id
                                 && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()));
                //Get Time work of employee
                var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60; //8.5 => 8h30
                                                                                                       //Get appointment detail => Check available time
                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                                    predicate: x => x.SalonEmployeeId == employee.Id
                                                    && x.StartTime.Date == request.Day.Date && x.StartTime <= DateBooking
                                                    && x.EndTime.Date == request.Day.Date && x.EndTime > DateBooking
                                                    && x.Status.Equals(AppointmentStatus.Booking));
                if (appointmentDetails == null)
                {
                    listEmp.Add(new EmployeeAvailable() { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                }
            }
            if (listEmp.Count == 0)
            {
                throw new NotFoundException($"Không có nhân viên nào có thể phụ vụ vào thời gian{DateBooking.ToString()}");
            }
            //Add list BookingDetail vào result
            bookingResponse.BookingDetailResponses.Add(new BookingDetailResponse()
            {
                ServiceHairId = firstBookingDetail.ServiceHairId,
                Employees = listEmp,
                StartTime = DateBooking,
                EndTime = DateBooking.AddHours((double)endTimeProcess),
                WaitingTime = 0
            });
            if (request.BookingDetail.Count > 1)
            {
                //***********************************
                for (int i = 1; i < request.BookingDetail.Count(); i++)
                {
                    var bookingDetail = request.BookingDetail[i];
                    if (bookingDetail.IsAnyOne)
                    {
                        //Get Serrvice Hair
                        serviceHair = await _unitOfWork.GetRepository<ServiceHair>()
                                                 .SingleOrDefaultAsync
                                                  (
                                                    predicate: x => x.Id == bookingDetail.ServiceHairId && x.SalonInformationId == request.SalonId
                                                  );
                        if (serviceHair == null)
                        {
                            throw new NotFoundException($"Không tìm thấy dịch vụ với id {bookingDetail.ServiceHairId} của salon id {request.SalonId} ");
                        }
                        //Get thời gian kết thúc sau khi thực hiện srv hair
                        endTimeProcess = endTimeProcess + serviceHair.Time; //8.25 
                        //check end time of schedule có đủ thời gian thực hiện srv hair không 
                        if (endTimeSalon < endTimeProcess)
                        {
                            throw new Exception("Thời gian thực hiện dịch vụ quá thời gian làm việc của salon, barber shop");
                        }
                        //Get List Employee can implement this service hair
                        listEmp = new List<EmployeeAvailable>();
                        if (bookingDetail.IsAnyOne)
                        {
                            //Xủ lý khi chọn employee nào cũng được => IsAnyOne = true
                            var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(
                                                            predicate: x => x.SalonInformationId == request.SalonId);
                            if (employees == null)
                            {
                                throw new NotFoundException("Không tìm thấy nhân viên của salon, barber shop");
                            }
                            foreach (var employee in employees)
                            {   // Get schedule by employee id
                                var scheduleEmp = await _unitOfWork.GetRepository<Schedule>()
                                                                   .SingleOrDefaultAsync
                                                                    (
                                                                        predicate: x => x.EmployeeId == employee.Id
                                                                                    && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString())
                                                                    );
                                //Get Time work of employee
                                var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                                var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60; //8.5 => 8h30
                                //Get appointment detail => Check available time
                                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                                          .GetListAsync
                                                                           (
                                                                                predicate: x => x.SalonEmployeeId == employee.Id
                                                                                            && x.StartTime.Date == request.Day.Date && x.StartTime <= DateBooking
                                                                                            && x.EndTime.Date == request.Day.Date && x.EndTime > DateBooking
                                                                                            && x.Status.Equals(AppointmentStatus.Booking)
                                                                           );
                                if (appointmentDetails == null)
                                {
                                    listEmp.Add(new EmployeeAvailable() { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                                }
                            }
                        }
                        else
                        {
                            //Xủ lý khi chọn employee cố định => IsAnyOne = false
                            var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(
                                            predicate: x => x.Id == firstBookingDetail.SalonEmployeeId);
                            if (employee == null)
                            {
                                throw new NotFoundException($"Không tìm thấy employee với id {firstBookingDetail.SalonEmployeeId}");
                            }
                            var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                            predicate: x => x.EmployeeId == employee.Id
                                             && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()));
                            //Get Time work of employee
                            var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                            var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60; //8.5 => 8h30
                                                                                                                   //Get appointment detail => Check available time
                            var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                                                predicate: x => x.SalonEmployeeId == employee.Id
                                                                && x.StartTime.Date == request.Day.Date && x.StartTime <= DateBooking
                                                                && x.EndTime.Date == request.Day.Date && x.EndTime > DateBooking
                                                                && x.Status.Equals(AppointmentStatus.Booking));
                            if (appointmentDetails == null)
                            {
                                listEmp.Add(new EmployeeAvailable() { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                            }
                        }
                        if (listEmp.Count == 0)
                        {
                            throw new NotFoundException($"Không có nhân viên nào có thể phụ vụ vào thời gian{DateBooking.ToString()}");
                        }
                        //Add list BookingDetail vào result
                        bookingResponse.BookingDetailResponses.Add(new BookingDetailResponse()
                        {
                            ServiceHairId = firstBookingDetail.ServiceHairId,
                            Employees = listEmp,
                            StartTime = DateBooking,
                            EndTime = DateBooking.AddHours((double)endTimeProcess),
                            WaitingTime = 0
                        });
                    }
                }
            }
            bookingResponse.Day = request.Day;
            bookingResponse.SalonId = request.SalonId;
            bookingResponse.StartTime = new DateTime(request.Day.Year, request.Day.Month, request.Day.Day, ((int)request.AvailableSlot), (int)(request.AvailableSlot - (int)request.AvailableSlot) * 60, 0);
            return bookingResponse;
        }
        private async Task<bool> CheckAppointmentBooking(Guid SalonId, DateTime Day, Decimal TimeSlot, bool IsAnyOne)
        {
            if (IsAnyOne)
            {
                var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(
                                predicate: x => x.SalonInformationId == SalonId);
                if (employees == null)
                {
                    throw new NotFoundException("Không tìm thấy nhân viên của salon, barber shop");
                }

                foreach (var employee in employees)
                {
                    var appointmentDetail = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                             .GetListAsync
                                                              (
                                                                predicate: x => x.SalonEmployeeId == employee.Id && x.StartTime.Date == Day.Date
                                                              );
                }
            }

            return true;
        }
        private List<decimal> GenerateTimeSlot(decimal begin, decimal end, decimal step)
        {
            int count = (int)((end - begin) / step) + 1;
            List<decimal> array = new List<decimal>();
            for (int i = 0; i < count; i++)
            {
                array.Add(begin + i * step);
            }
            return array;
        }

        #endregion

        #region Create Update Delete Active
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
            appointment.Status = AppointmentStatus.Booking;
            appointment.Date = DateTime.Now;
            await _unitOfWork.GetRepository<Appointment>().InsertAsync(appointment);
            await _unitOfWork.CommitAsync();
            if (createAccountRequest.ListAppointmentDetail == null || createAccountRequest.ListAppointmentDetail.Count == 0)
            {
                throw new NotFoundException("AppointmentDetail not found!");
            }
            foreach (var item in createAccountRequest.ListAppointmentDetail)
            {
                try
                {
                    await _appointmentDetailService.CreateAppointmentDetailFromAppointment(appointment.Id, item);
                }
                catch (NotFoundException ex)
                {
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
            appoinment.Status = AppointmentStatus.Fail;
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
            appoinment.Status = AppointmentStatus.Booking;
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
        #endregion

    }
}
