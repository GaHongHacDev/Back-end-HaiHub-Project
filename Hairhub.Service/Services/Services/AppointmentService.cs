using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Hairhub.Common.CommonService.Contract;
using System.Data;

namespace Hairhub.Service.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentDetailService _appointmentDetailService;
        private readonly IQRCodeService _qrCodeService;
        private readonly IEmailService _emailService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentDetailService appointmentDetailService, IQRCodeService qrCodeService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentDetailService = appointmentDetailService;
            _qrCodeService = qrCodeService;
            _emailService = emailService;
        }

        #region GET

        public async Task<IPaginate<GetAppointmentResponse>> GetAllAppointment(int page, int size)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
           .GetPagingListAsync(
              include: query => query.Include(a => a.Customer)
                                    .Include(a => a.AppointmentDetails)
                                        .ThenInclude(ad => ad.SalonEmployee)
                                            .ThenInclude(se => se.SalonInformation),
               page: page,
               size: size
           );

            var appointmentResponse = new Paginate<GetAppointmentResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(appointments.Items),
            };

            if (appointmentResponse != null && appointmentResponse.Items.Count != 0)
            {
                foreach (var item in appointmentResponse.Items)
                {
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                        .GetListAsync(predicate: x => x.AppointmentId == item.Id, include: x => x.Include(x => x.SalonEmployee).Include(y => y.ServiceHair));
                    item.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
                }
            }
            return appointmentResponse;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetHistoryAppointmentByCustomerId(int page, int size, Guid CustomerId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                               .GetPagingListAsync(
                                                   predicate: x => x.CustomerId == CustomerId && (x.Status.Equals(AppointmentStatus.Successed)
                                                              || x.Status.Equals(AppointmentStatus.CancelByCustomer)
                                                              ),
                                                   include: query => query.Include(a => a.Customer)
                                                                           .Include(a => a.AppointmentDetails)
                                                                                .ThenInclude(ad => ad.SalonEmployee)
                                                                                    .ThenInclude(se => se.SalonInformation),
                                                   page: page,
                                                   size: size
                                               );

            var appointmentResponse = new Paginate<GetAppointmentResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(appointments.Items),
            };
            if (appointmentResponse != null && appointmentResponse.Items.Count > 0)
            {
                foreach (var item in appointmentResponse.Items)
                {
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                        .GetListAsync(predicate: x => x.AppointmentId == item.Id, include: x => x.Include(x => x.SalonEmployee).Include(y => y.ServiceHair));
                    item.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
                }
            }
            return appointmentResponse;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetBookingAppointmentByCustomerId(int page, int size, Guid CustomerId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                               .GetPagingListAsync(
                                                   predicate: x => x.CustomerId == CustomerId && x.Status.Equals(AppointmentStatus.Booking),
                                                   include: query => query.Include(a => a.Customer)
                                                                           .Include(a => a.AppointmentDetails)
                                                                               .ThenInclude(ad => ad.SalonEmployee)
                                                                                   .ThenInclude(se => se.SalonInformation),
                                                   page: page,
                                                   size: size
                                               );
            var appointmentResponse = new Paginate<GetAppointmentResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(appointments.Items),
            };
            if (appointmentResponse != null && appointmentResponse.Items.Count != 0)
            {
                foreach (var item in appointmentResponse.Items)
                {
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                        .GetListAsync(predicate: x => x.AppointmentId == item.Id, include: x => x.Include(x => x.SalonEmployee).Include(y => y.ServiceHair));
                    item.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
                }
            }
            return appointmentResponse;
        }

        public async Task<GetAppointmentResponse>? GetAppointmentById(Guid id)
        {
            Appointment appointment = await _unitOfWork
                .GetRepository<Appointment>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: query => query.Include(a => a.Customer)
                                            .Include(a => a.AppointmentDetails)
                                                .ThenInclude(ad => ad.SalonEmployee)
                                                    .ThenInclude(se => se.SalonInformation)
                 );
            if (appointment == null)
                return null;
            var appointmentResponse = _mapper.Map<GetAppointmentResponse>(appointment);
            var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                .GetListAsync(predicate: x => x.AppointmentId == appointmentResponse.Id, include: x => x.Include(x => x.SalonEmployee).Include(y => y.ServiceHair));
            appointmentResponse.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
            return appointmentResponse;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetAppointmentByAccountId(Guid AccountId, int page, int size)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.AccountId == AccountId);
            if (customer == null)
            {
                throw new NotFoundException($"Not found customer with id {AccountId}");
            }
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetPagingListAsync(
                    predicate: x => x.CustomerId == customer.Id && x.Status.Equals(AppointmentStatus.Booking),
                    include: query => query.Include(a => a.Customer)
                                           .Include(a => a.AppointmentDetails)
                                               .ThenInclude(ad => ad.SalonEmployee)
                                                   .ThenInclude(se => se.SalonInformation),
                    page: page,
                    size: size
                );
            var appointmentResponse = new Paginate<GetAppointmentResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(appointments.Items),
            };
            foreach (var item in appointmentResponse.Items)
            {
                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                    .GetListAsync(predicate: x => x.AppointmentId == item.Id, include: x => x.Include(x => x.SalonEmployee).Include(y => y.ServiceHair));
                item.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
            }
            return appointmentResponse;
        }
        public async Task<IPaginate<GetAppointmentResponse>> GetAppointmentSalonByStatus(int page, int size, Guid salonId, string? status)
        {
            // Tạo biểu thức điều kiện ban đầu cho SalonId
            var predicate = PredicateBuilder.New<Appointment>(x => x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId));
            if (!string.IsNullOrEmpty(status))
            {
                predicate = predicate.And(x => x.Status.Equals(status));
            }
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetPagingListAsync(
                    predicate: predicate,
                    include: query => query.Include(a => a.Customer)
                                           .Include(a => a.AppointmentDetails)
                                               .ThenInclude(ad => ad.SalonEmployee)
                                                   .ThenInclude(se => se.SalonInformation),
                    page: page,
                    size: size
                );
            var appointmentResponse = new Paginate<GetAppointmentResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(appointments.Items),
            };
            if (appointmentResponse != null && appointmentResponse.Items.Count > 0)
            {
                foreach (var item in appointmentResponse.Items)
                {
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                        .GetListAsync(predicate: x => x.AppointmentId == item.Id, include: x => x.Include(x => x.SalonEmployee).Include(y => y.ServiceHair));
                    item.AppointmentDetails = _mapper.Map<List<GetAppointmentDetailResponse>>(appointmentDetails);
                }
            }
            return appointmentResponse;
        }


        public async Task<IPaginate<GetAppointmentResponse>> GetAppointmentEmployeeByStatus(int page, int size, Guid EmployeeId, string? Status)
        {
            // Tạo biểu thức điều kiện ban đầu cho employeeId
            var predicate = PredicateBuilder.New<Appointment>(x => x.AppointmentDetails.Any(ad => ad.SalonEmployee.Id == EmployeeId));
            if (!string.IsNullOrEmpty(Status))
            {
                predicate = predicate.And(x => x.Status.Equals(Status));
            }
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                  .GetPagingListAsync(
                        predicate: predicate,
                        include: query => query.Include(a => a.Customer)
                                               .Include(a => a.AppointmentDetails)
                                                   .ThenInclude(ad => ad.SalonEmployee)
                                                   .ThenInclude(se => se.SalonInformation)
                                               .Include(a => a.AppointmentDetails)
                                                   .ThenInclude(ad => ad.ServiceHair),
                        page: page,
                        size: size
                  );
            var appointmentResponse = new Paginate<GetAppointmentResponse>()
            {
                Page = appointments.Page,
                Size = appointments.Size,
                Total = appointments.Total,
                TotalPages = appointments.TotalPages,
                Items = _mapper.Map<IList<GetAppointmentResponse>>(appointments.Items),
            };
            return appointmentResponse;
        }

        #endregion

        #region Booking

        public async Task<GetAvailableTimeResponse> GetAvailableTime(GetAvailableTimeRequest request)
        {
            GetAvailableTimeResponse result = new GetAvailableTimeResponse();
            var salonSchedule = await _unitOfWork.GetRepository<Schedule>()
                                                       .SingleOrDefaultAsync(predicate: x => x.SalonId == request.SalonId
                                                                                  && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString())
                                                                                  && x.IsActive);
            if (salonSchedule == null)
            {
                throw new NotFoundException("Salon, barber shop không hoạt động vào thời gian này");
            }
            //****************************************
            /* if (salonSchedule.EndTime)
             {

             }*/
            List<decimal> availbeTimeResponse = GenerateTimeSlot(salonSchedule.StartTime.Hour + (decimal)salonSchedule.StartTime.Minute / 60, salonSchedule.EndTime.Hour + (decimal)salonSchedule.EndTime.Minute / 60, 0.25m);
            var availableTimesDict = availbeTimeResponse.ToDictionary(timeSlot => timeSlot, timeSlot => new List<EmployeeAvailable>());
            if (!request.IsAnyOne)
            {
                // Get all employees in the salon
                var employee = await _unitOfWork.GetRepository<SalonEmployee>()
                                                 .SingleOrDefaultAsync(predicate: x => x.Id == request.SalonEmployeeId && x.IsActive);

                if (employee == null)
                {
                    throw new NotFoundException("Salon hiện không có nhân viên làm việc");
                }

                var tempAvailableTimes = new Dictionary<decimal, List<EmployeeAvailable>>();

                // Get schedule by id
                var scheduleEmp = await _unitOfWork.GetRepository<Schedule>()
                                                   .SingleOrDefaultAsync(predicate: x => x.EmployeeId == employee.Id
                                                                              && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString())
                                                                              && x.IsActive);
                if (scheduleEmp == null)
                {
                    throw new NotFoundException("Không tìm thấy lịch làm việc của nhân viên");
                }

                var serviceEmployee = await _unitOfWork.GetRepository<ServiceEmployee>()
                                                       .SingleOrDefaultAsync(predicate: x => x.ServiceHairId == request.ServiceHairId
                                                                                  && x.SalonEmployeeId == employee.Id
                                                                                  && x.ServiceHair.IsActive,
                                                                            include: x => x.Include(y => y.ServiceHair));
                if (serviceEmployee == null) // Employee không thực hiện srv này => Continue
                {
                    throw new NotFoundException("Nhân viên không phục vụ cho dịch vụ này");
                }

                // Get time work of employee
                var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60 - serviceEmployee.ServiceHair.Time;

                // Define list time work of employee
                List<decimal> timeSlotEmployee = GenerateTimeSlot(startSchedule, endSchedule, 0.25m);

                // Get appointment detail => Check available time
                var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                          .GetListAsync(predicate: x => x.SalonEmployeeId == employee.Id
                                                                             && x.StartTime.Date == request.Day.Date
                                                                             && x.EndTime.Date == request.Day.Date
                                                                             && x.Status.Equals(AppointmentStatus.Booking));

                foreach (var item in appointmentDetails)
                {
                    decimal start = ParseTimeToDecimal(item.StartTime);
                    decimal end = ParseTimeToDecimal(item.EndTime);
                    timeSlotEmployee.RemoveAll(slot => slot >= start && slot < end);
                }

                foreach (var timeSlot in timeSlotEmployee)
                {
                    if (availableTimesDict.ContainsKey(timeSlot))
                    {
                        if (!availableTimesDict[timeSlot].Any(ea => ea.Id == employee.Id))
                        {
                            availableTimesDict[timeSlot].Add(new EmployeeAvailable { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                        }
                    }
                }
                // Loại bỏ các thời gian không có nhân viên nào
                availableTimesDict = availableTimesDict
                    .Where(kvp => kvp.Value.Any())
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                if (availableTimesDict == null || availableTimesDict.Count == 0)
                {
                    throw new NotFoundException("Không có nhân viên nào có thể làm việc vào thời gian này");
                }
                // Convert dictionary to List<AvailableTime>
                result.AvailableTimes = availableTimesDict.Select(kvp => new AvailableTime
                {
                    TimeSlot = kvp.Key,
                    employeeAvailables = kvp.Value
                }).ToList();
            }
            else if (request.IsAnyOne) //Xủ lý khi chọn employee nào cũng được
            {
                // Get all employees in the salon
                var employees = await _unitOfWork.GetRepository<SalonEmployee>()
                                                 .GetListAsync(predicate: x => x.SalonInformationId == request.SalonId && x.IsActive);

                if (employees == null)
                {
                    throw new NotFoundException("Salon hiện không có nhân viên làm việc");
                }

                var tempAvailableTimes = new Dictionary<decimal, List<EmployeeAvailable>>();

                foreach (var employee in employees)
                {
                    // Get schedule by id
                    var scheduleEmp = await _unitOfWork.GetRepository<Schedule>()
                                                       .SingleOrDefaultAsync(predicate: x => x.EmployeeId == employee.Id
                                                                                  && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString())
                                                                                  && x.IsActive);
                    if (scheduleEmp == null)
                    {
                        continue;
                    }

                    var serviceEmployee = await _unitOfWork.GetRepository<ServiceEmployee>()
                                                           .SingleOrDefaultAsync(predicate: x => x.ServiceHairId == request.ServiceHairId
                                                                                      && x.SalonEmployeeId == employee.Id
                                                                                      && x.ServiceHair.IsActive,
                                                                                include: x => x.Include(y => y.ServiceHair));
                    if (serviceEmployee == null) // Employee không thực hiện srv này => Continue
                    {
                        continue;
                    }

                    // Get time work of employee
                    var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                    var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60 - serviceEmployee.ServiceHair.Time;

                    // Define list time work of employee
                    List<decimal> timeSlotEmployee = GenerateTimeSlot(startSchedule, endSchedule, 0.25m);

                    // Get appointment detail => Check available time
                    var appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                              .GetListAsync(predicate: x => x.SalonEmployeeId == employee.Id
                                                                                 && x.StartTime.Date == request.Day.Date
                                                                                 && x.EndTime.Date == request.Day.Date
                                                                                 && x.Status.Equals(AppointmentStatus.Booking));

                    foreach (var item in appointmentDetails)
                    {
                        decimal start = ParseTimeToDecimal(item.StartTime);
                        decimal end = ParseTimeToDecimal(item.EndTime);
                        await Console.Out.WriteLineAsync(start + " : " + end);
                        timeSlotEmployee.RemoveAll(slot => slot >= start && slot < end);
                    }

                    foreach (var timeSlot in timeSlotEmployee)
                    {
                        if (availableTimesDict.ContainsKey(timeSlot))
                        {
                            if (!availableTimesDict[timeSlot].Any(ea => ea.Id == employee.Id))
                            {
                                availableTimesDict[timeSlot].Add(new EmployeeAvailable { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                            }
                        }
                    }
                }

                // Loại bỏ các thời gian không có nhân viên nào
                availableTimesDict = availableTimesDict
                    .Where(kvp => kvp.Value.Any())
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                if (availableTimesDict == null || availableTimesDict.Count == 0)
                {
                    throw new NotFoundException("Không có nhân viên nào có thể làm việc vào thời gian này");
                }
                // Convert dictionary to List<AvailableTime>
                result.AvailableTimes = availableTimesDict.Select(kvp => new AvailableTime
                {
                    TimeSlot = kvp.Key,
                    employeeAvailables = kvp.Value
                }).ToList();
            }
            return result;
        }

        public async Task<BookAppointmentResponse> BookAppointment(BookAppointmentRequest request)
        {
            BookAppointmentResponse bookingResponse = new BookAppointmentResponse();
            Decimal? endTimeProcess = null;
            Decimal startTimeProcess;
            DateTime StartTimeBooking = new DateTime(request.Day.Year, request.Day.Month, request.Day.Day, (int)request.AvailableSlot, (int)((request.AvailableSlot - (int)request.AvailableSlot) * 60), 0);
            var scheduleSolon = await _unitOfWork.GetRepository<Schedule>()
                                 .SingleOrDefaultAsync
                                  (
                                    predicate: x => x.SalonId == request.SalonId && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()) && x.IsActive == true
                                  );
            if (scheduleSolon == null)
            {
                throw new NotFoundException($"Salon, barber shop không hoạt động vào {(int)request.AvailableSlot}:{(int)((request.AvailableSlot - (int)request.AvailableSlot) * 60)}");
            }
            Decimal endTimeSalon = scheduleSolon.EndTime.Hour + (scheduleSolon.EndTime.Minute) / 60m;
            List<EmployeeAvailable> listEmp = new List<EmployeeAvailable>();
            Decimal waitingTime = 0;
            for (int i = 0; i < request.BookingDetail.Count(); i++)
            {
                var bookingDetail = request.BookingDetail[i];
                //Get Serrvice Hair
                var serviceHair = await _unitOfWork.GetRepository<ServiceHair>()
                                         .SingleOrDefaultAsync
                                          (
                                            predicate: x => x.Id == bookingDetail.ServiceHairId && x.SalonInformationId == request.SalonId && x.IsActive == true
                                          );
                if (serviceHair == null)
                {
                    throw new NotFoundException($"Không tìm thấy dịch vụ với id {bookingDetail.ServiceHairId} của salon id {request.SalonId} ");
                }
                //Get thời gian kết thúc sau khi thực hiện srv hair
                startTimeProcess = endTimeProcess ??= request.AvailableSlot;
                endTimeProcess = startTimeProcess + serviceHair.Time;
                //check end time of schedule có đủ thời gian thực hiện srv hair không 
                if (endTimeSalon < endTimeProcess)
                {
                    throw new Exception("Thời gian thực hiện dịch vụ quá thời gian làm việc của salon, barber shop");
                }
                //******************
                listEmp = await CaculateBookingDetail(bookingDetail, request, startTimeProcess, endTimeProcess);
                if (listEmp.Count == 0 && i == 0)
                {
                    if (i == 0)
                    {
                        throw new NotFoundException($"Không tìm thấy nhân viên phục vụ cho dịch vụ thứ nhất - {serviceHair.ServiceName} vào {(int)request.AvailableSlot}:{(int)((request.AvailableSlot - (int)request.AvailableSlot) * 60)}");
                    }
                    else
                    {
                        throw new NotFoundException($"Không tìm thấy nhân viên phục vụ cho dịch vụ thứ {i + 1} - {serviceHair.ServiceName}");
                    }
                }
                // Caculate waiting time for another time in services > 1
                if (listEmp.Count == 0 && i > 0)
                {
                    for (decimal j = startTimeProcess + 0.25m; j <= endTimeSalon - serviceHair.Time; j += 0.25m)
                    {
                        listEmp = await CaculateBookingDetail(bookingDetail, request, j, j + serviceHair.Time);
                        if (listEmp.Count != 0)
                        {
                            waitingTime = j - startTimeProcess;
                            break;
                        }
                    }
                    if (listEmp.Count == 0)
                    {
                        throw new Exception($"Không đủ thời gian hoặc thiếu nhân viên để thực hiện dịch vụ thứ {i + 1}");
                    }
                }
                var serviceHairResult = _mapper.Map<ServiceHairAvalibale>(serviceHair);
                serviceHairResult.StartTime = StartTimeBooking;
                StartTimeBooking = StartTimeBooking.AddHours((double)serviceHair.Time);
                serviceHairResult.EndTime = StartTimeBooking;
                serviceHairResult.WaitingTime = waitingTime;
                //Add list BookingDetail vào result
                bookingResponse.BookingDetailResponses.Add(new BookingDetailResponse()
                {
                    ServiceHair = serviceHairResult,
                    Employees = listEmp,
                });
            }
            bookingResponse.Day = request.Day;
            bookingResponse.SalonId = request.SalonId;
            bookingResponse.StartTime = new DateTime(request.Day.Year, request.Day.Month, request.Day.Day, ((int)request.AvailableSlot), (int)(request.AvailableSlot - (int)request.AvailableSlot) * 60, 0);
            return bookingResponse;
        }

        private async Task<List<EmployeeAvailable>> CaculateBookingDetail(BookingDetailRequest bookingDetail, BookAppointmentRequest request, Decimal startTimeProcess, Decimal? endTimeProcess)
        {
            List<EmployeeAvailable> listEmp = new List<EmployeeAvailable>();
            if (bookingDetail.IsAnyOne)
            {
                //Xủ lý khi chọn employee nào cũng được => IsAnyOne = true
                var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync
                                                  (
                                                    predicate: x => x.SalonInformationId == request.SalonId &&
                                                                    x.ServiceEmployees.Any(se => se.ServiceHairId == bookingDetail.ServiceHairId)
                                                                    && x.IsActive == true
                                                  );
                if (employees == null)
                {
                    throw new NotFoundException("Không tìm thấy nhân viên của salon, barber shop có thể phục vụ dịch vụ này");
                }
                foreach (var employee in employees)
                {   // Get schedule by id
                    var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                    predicate: x => x.EmployeeId == employee.Id
                                     && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()) && x.IsActive == true);
                    if (scheduleEmp == null)
                    {
                        continue;
                    }
                    //Get Time work of employee
                    var startScheduleEmp = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                    var endScheduleEmp = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60; //8.5 => 8h30
                    if (startTimeProcess >= startScheduleEmp && endTimeProcess <= endScheduleEmp)
                    {
                        //Get appointment detail => Check available time
                        var appointmentDetails = (await _unitOfWork.GetRepository<AppointmentDetail>()
                                                        .GetListAsync(predicate: x => x.SalonEmployeeId == employee.Id
                                                                               && x.StartTime.Date == request.Day.Date
                                                                               && x.EndTime.Date == request.Day.Date
                                                                               && x.Status.Equals(AppointmentStatus.Booking)))
                                                        .ToList()
                                                        .Where(a => ParseTimeToDecimal(a.StartTime) <= startTimeProcess && ParseTimeToDecimal(a.EndTime) > startTimeProcess
                                                                 || (decimal?)ParseTimeToDecimal(a.StartTime) < endTimeProcess && (decimal?)ParseTimeToDecimal(a.EndTime) >= endTimeProcess
                                                                 || ParseTimeToDecimal(a.StartTime) > startTimeProcess && (decimal?)ParseTimeToDecimal(a.StartTime) < endTimeProcess)
                                                        .ToList();
                        if (appointmentDetails == null || appointmentDetails.Count == 0)
                        {
                            listEmp.Add(new EmployeeAvailable() { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                        }
                    }
                }
            }
            else
            {
                //Xủ lý khi chọn employee cố định => IsAnyOne = false
                var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(
                                predicate: x => x.Id == bookingDetail.SalonEmployeeId);
                if (employee == null)
                {
                    throw new NotFoundException($"Không tìm thấy employee với id {bookingDetail.SalonEmployeeId}");
                }
                // Get schedule by id
                var scheduleEmp = await _unitOfWork.GetRepository<Schedule>().SingleOrDefaultAsync(
                                predicate: x => x.EmployeeId == employee.Id
                                 && x.DayOfWeek.Equals(request.Day.DayOfWeek.ToString()));
                //Get Time work of employee
                var startScheduleEmp = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                var endScheduleEmp = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60; //8.5 => 8h30
                if (startTimeProcess >= startScheduleEmp && endTimeProcess <= endScheduleEmp)
                {
                    //Get appointment detail => Check available time
                    var appointmentDetails = (await _unitOfWork.GetRepository<AppointmentDetail>()
                                                    .GetListAsync(predicate: x => x.SalonEmployeeId == employee.Id
                                                                           && x.StartTime.Date == request.Day.Date
                                                                           && x.EndTime.Date == request.Day.Date
                                                                           && x.Status.Equals(AppointmentStatus.Booking)))
                                                    .ToList()
                                                    .Where(a => ParseTimeToDecimal(a.StartTime) <= startTimeProcess && ParseTimeToDecimal(a.EndTime) > startTimeProcess
                                                             || (decimal?)ParseTimeToDecimal(a.StartTime) < endTimeProcess && (decimal?)ParseTimeToDecimal(a.EndTime) >= endTimeProcess
                                                             || ParseTimeToDecimal(a.StartTime) > startTimeProcess && (decimal?)ParseTimeToDecimal(a.StartTime) < endTimeProcess)
                                                    .ToList();
                    if (appointmentDetails == null || appointmentDetails.Count == 0)
                    {
                        listEmp.Add(new EmployeeAvailable() { Id = employee.Id, FullName = employee.FullName, Img = employee.Img });
                    }
                }
            }
            return listEmp;
        }

        private Decimal ParseTimeToDecimal(DateTime Time)
        {
            if (Time.Hour == null)
            {
                throw new Exception($"Cannot convert hour {Time}");
            }
            if (Time.Minute == null)
            {
                throw new Exception($"Cannot convert minute {Time}");
            }
            return (decimal)(Time.Hour + Time.Minute / 60m);
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
        public async Task<bool> CreateAppointment(CreateAppointmentRequest request)
        {
            Guid id = Guid.NewGuid();
            string url = await _qrCodeService.GenerateQR(id);
            if (String.IsNullOrEmpty(url))
            {
                throw new Exception("Lỗi không thể tạo QR check in cho đơn đặt lịch này");
            }
            var appointment = new Appointment()
            {
                Id = id,
                CustomerId = request.CustomerId,
                CreatedDate = DateTime.Now,
                StartDate = request.StartDate,
                TotalPrice = request.TotalPrice,
                OriginalPrice = request.OriginalPrice,
                DiscountedPrice = request.DiscountedPrice,
                Status = AppointmentStatus.Booking,
                QrCodeImg = url,
            };
            await _unitOfWork.GetRepository<Appointment>().InsertAsync(appointment);

            if (request.AppointmentDetails == null || request.AppointmentDetails.Count == 0)
            {
                throw new NotFoundException("Không tìm thấy đơn đặt lịch");
            }
            foreach (var item in request.AppointmentDetails)
            {
                await _appointmentDetailService.CreateAppointmentDetailFromAppointment(appointment.Id, item);
            }
            if (request.VoucherIds != null && request.VoucherIds.Count > 0)
            {
                foreach (var item in request.VoucherIds)
                {
                    var voucher = await _unitOfWork.GetRepository<Voucher>()
                                             .SingleOrDefaultAsync
                                             (
                                                predicate: x => x.Id == item
                                                            && x.IsActive == true
                                             );
                    if (voucher == null)
                    {
                        throw new NotFoundException("Voucher Không phù hợp");
                    }
                    var appointmentVoucher = new AppointmentDetailVoucher()
                    {
                        Id = new Guid(),
                        AppointmentId = appointment.Id,
                        VoucherId = item
                    };
                    await _unitOfWork.GetRepository<AppointmentDetailVoucher>().InsertAsync(appointmentVoucher);
                }
            }
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            return isInsert;
        }

        public async Task<bool> UpdateAppointmentById(Guid id, UpdateAppointmentRequest updateAppointmentRequest)
        {
            var appoinment = await _unitOfWork.GetRepository<Appointment>()
                                              .SingleOrDefaultAsync
                                              (
                                                predicate: x => x.Id == id && updateAppointmentRequest.CustomerId == x.CustomerId,
                                                include: x => x.Include(x => x.AppointmentDetails)
                                              );
            if (appoinment == null)
            {
                throw new NotFoundException("Không tìm thấy đơn đặt lịch");
            }

            if (!updateAppointmentRequest.Status.Equals(AppointmentStatus.Successed) && !updateAppointmentRequest.Status.Equals(AppointmentStatus.Fail)
                && !updateAppointmentRequest.Status.Equals(AppointmentStatus.CancelByCustomer)
                && !updateAppointmentRequest.Status.Equals(AppointmentStatus.Booking))
            {
                throw new Exception("Status không tồn tại");
            }
            foreach (var item in appoinment.AppointmentDetails)
            {
                item.Status = updateAppointmentRequest.Status;
                _unitOfWork.GetRepository<AppointmentDetail>().UpdateAsync(item);
            }

            appoinment.Status = updateAppointmentRequest.Status;
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

        public async Task<GetCalculatePriceResponse> CalculatePrice(GetCalculatePriceRequest calculatePriceRequest)
        {



            decimal discountPercentage = 0;
            decimal originalPriceService = 0;

            if (calculatePriceRequest.VoucherId.HasValue)
            {
                var existingVoucher = await _unitOfWork.GetRepository<Voucher>().SingleOrDefaultAsync(predicate: e => e.Id == calculatePriceRequest.VoucherId.Value);
                if (existingVoucher == null)
                {
                    throw new NotFoundException("Voucher Not Found");
                }
                discountPercentage = existingVoucher.DiscountPercentage;
            }

            var serviceHairIds = calculatePriceRequest.ServiceHairId;
            var services = await _unitOfWork.GetRepository<ServiceHair>().GetListAsync(predicate: s => serviceHairIds.Contains(s.Id));

            foreach (var service in services)
            {
                var finalPrice = service.Price;
                if (finalPrice > 0)
                {
                    originalPriceService += finalPrice;
                }
            }

            decimal discountedPrice = discountPercentage * originalPriceService;
            decimal totalPrice = originalPriceService - discountedPrice;

            var response = new GetCalculatePriceResponse
            {
                OriginalPrice = originalPriceService,
                DiscountedPrice = discountedPrice,
                TotalPrice = totalPrice,
            };

            return response;

        }

        public async Task<bool> CancelAppointmentByCustomer(Guid id, CancelApointmentRequest cancelApointmentRequest)
        {

            var appoinment = await _unitOfWork.GetRepository<Appointment>()
                                                         .SingleOrDefaultAsync
                                                         (
                                                           predicate: x => x.Id == id && cancelApointmentRequest.CustomerId == x.CustomerId,
                                                           include: x => x.Include(x => x.AppointmentDetails).ThenInclude(s => s.SalonEmployee).ThenInclude(s => s.SalonInformation).ThenInclude(s => s.SalonOwner)
                                                                          .Include(s=>s.Customer)
                                                         );

            if (appoinment == null)
            {
                throw new NotFoundException("Không tìm thấy đơn đặt lịch");
            }
            else
            {
                if (!appoinment.Status.Equals(AppointmentStatus.Booking))
                {
                    throw new Exception("Chỉ được hủy lịch cắt tóc khi đã đặt lịch hẹn");
                }
            }

            foreach (var item in appoinment.AppointmentDetails)
            {
                item.Status = AppointmentStatus.CancelByCustomer;
                _unitOfWork.GetRepository<AppointmentDetail>().UpdateAsync(item);
            }

            appoinment.Status = AppointmentStatus.CancelByCustomer;
            appoinment.ReasonCancel = cancelApointmentRequest.reasonCancel;
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);

            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            if (isUpdate)
            {
                DateTime TimeBook = appoinment.AppointmentDetails.OrderBy(s => s.StartTime).FirstOrDefault().StartTime;
                SalonInformation Salon = appoinment.AppointmentDetails.FirstOrDefault().SalonEmployee.SalonInformation;
                string bodyEmail = $"Chúng tôi rất tiếc phải thông báo rằng khách hàng {appoinment.Customer.FullName} của bạn đã hủy lịch hẹn cắt tóc có thời gian vào {TimeBook.Hour}:{TimeBook.Minute} ngày {appoinment.StartDate.Day}, tháng {appoinment.StartDate.Month}, năm {appoinment.StartDate.Year}. Lý do: {cancelApointmentRequest.reasonCancel}. Vui lòng kiểm tra lại lịch trình của bạn để biết thêm chi tiết";
                await _emailService.SendEmailWithBodyAsync(Salon.SalonOwner.Email, "Thông báo hủy đơn đặt lịch trên Hairhub", Salon.Name, bodyEmail);
            }
            return isUpdate;
        }

        #endregion

    }
}
