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
using MailKit.Search;
using Hairhub.Domain.Dtos.Responses.Dashboard;
using Hairhub.Common.ThirdParties.Contract;
using System;



namespace Hairhub.Service.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentDetailService _appointmentDetailService;
        private readonly IQRCodeService _qrCodeService;
        private readonly IEmailService _emailService;
        private readonly IMediaService _mediaService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentDetailService appointmentDetailService,
                                    IQRCodeService qrCodeService, IEmailService emailService, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointmentDetailService = appointmentDetailService;
            _qrCodeService = qrCodeService;
            _emailService = emailService;
            _mediaService = mediaService;
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
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
                }
            }
            return appointmentResponse;
        }

        public async Task<GetAppointmentTransactionResponse> GetAppointmentTransaction(Guid salonId, DateTime startDate, DateTime endDate)
        {
            var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == salonId);
            if (salon == null)
            {
                throw new NotFoundException("Không tìm thấy salon, barber shop");
            }
            var predicate = PredicateBuilder.New<Appointment>(x => x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId));
            predicate = predicate.And(x => startDate.Date <= x.StartDate.Date && endDate.Date>=x.StartDate.Date);

            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                                .GetListAsync
                                                (
                                                    predicate: predicate,
                                                    orderBy: x => x.OrderByDescending(x => x.StartDate)
                                                );
            GetAppointmentTransactionResponse response = new GetAppointmentTransactionResponse();
            var payment = await _unitOfWork.GetRepository<Payment>()
                                            .SingleOrDefaultAsync(predicate: p => p.SalonOWnerID == salon.OwnerId && p.Status == PaymentStatus.Fake, orderBy: x => x.OrderByDescending(s => s.StartDate));
            if (payment == null)
            {
                throw new NotFoundException("Không tìm thấy thông tin thanh toán");
            }
            if (appointments != null)
            {
                int canceledAppointmentCount = 0;
                int successedAppointmentCount = 0;
                int failedAppointmentCount = 0;
                List<Appointment> appointmentsResponse = new List<Appointment>();

                foreach (var appointment in appointments)
                {
                    switch (appointment.Status)
                    {
                        case AppointmentStatus.Successed:
                            appointmentsResponse.Add(appointment);
                            //Tính tiền HH mà salon chưa trả cho system
                            if (appointment.StartDate >= payment.StartDate && appointment.StartDate <= payment.EndDate)
                            {
                                response.CurrentComssion += (appointment.CommissionRate / 100) * appointment.TotalPrice;
                            }
                            //Tính tổng tiền HH của salon từ start_date đến end_date
                            response.TotalComssion += (appointment.CommissionRate / 100) * appointment.TotalPrice;
                            //Tổng appointment thành công
                            successedAppointmentCount++;
                            break;
                        case AppointmentStatus.Fail:
                            //Tổng appointment thất bại
                            failedAppointmentCount++;
                            break;
                        case AppointmentStatus.CancelByCustomer:
                            //Tổng appointment bị khách hàng hủy
                            canceledAppointmentCount++;
                            break;
                    }
                }
                response.CanceledAppointmentCount = canceledAppointmentCount;
                response.SuccessedAppointmentCount = successedAppointmentCount;
                response.FailedAppointmentCount = failedAppointmentCount;
                response.AppointmentTransactions = _mapper.Map<List<AppointmentTransaction>>(appointmentsResponse);
            }
            return response;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetHistoryAppointmentByCustomerId(int page, int size, Guid CustomerId)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                               .GetPagingListAsync(
                                                   predicate: x => x.CustomerId == CustomerId && (x.Status.Equals(AppointmentStatus.Successed)
                                                              || x.Status.Equals(AppointmentStatus.CancelByCustomer)
                                                              || x.Status.Equals(AppointmentStatus.Fail)
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
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
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
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
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
            appointmentResponse.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == appointmentResponse.Id && x.IsActive == true) != null;
            return appointmentResponse;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetAppointmentByAccountId(Guid AccountId, int page, int size)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.AccountId == AccountId);
            if (customer == null)
            {
                throw new NotFoundException($"Không tìm thấy id của khách hàng");
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
                item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
            }
            return appointmentResponse;
        }

        public async Task<List<GetAppointmentResponse>> GetAppointmentSalonBySalonIdNoPaging(Guid salonId)
        {

            // Tạo biểu thức điều kiện ban đầu cho SalonId
            var predicate = PredicateBuilder.New<Appointment>(x => x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId));
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetListAsync(
                    predicate: predicate,
                    include: query => query.Include(a => a.Customer)
                                           .Include(a => a.AppointmentDetails)
                                               .ThenInclude(ad => ad.SalonEmployee)
                                                   .ThenInclude(se => se.SalonInformation)
                );
            var appointmentResponse = _mapper.Map<List<GetAppointmentResponse>>(appointments);

            if (appointmentResponse != null && appointmentResponse.Count > 0)
            {
                foreach (var item in appointmentResponse)
                {
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
                }
            }
            return appointmentResponse;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetAppointmentSalonByStatus(int page, int size, Guid salonId, string? status, bool isAscending, DateTime? date, string? customerName)
        {
            ExpressionStarter<Appointment> predicate;
            if (date.HasValue)
            {
                predicate = PredicateBuilder.New<Appointment>(x => x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId) && x.StartDate.Date == date.Value.Date);

            }
            else
            {
                predicate = PredicateBuilder.New<Appointment>(x => x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId));
            }

            if (customerName != null)
            {
                customerName = customerName.Trim();
                predicate = predicate.And(x => x.Customer.FullName.ToUpper().Contains(customerName.ToUpper()));
            }
            if (status == null)
            {
                status = "";
            }
            predicate = predicate.And(x => x.Status.Contains(status));
            IPaginate<Appointment> appointments;
            if (isAscending)
            {
                appointments = await _unitOfWork.GetRepository<Appointment>()
                    .GetPagingListAsync(
                        predicate: predicate,
                        include: query => query.Include(a => a.Customer)
                                               .Include(a => a.AppointmentDetails)
                                                   .ThenInclude(ad => ad.SalonEmployee)
                                                       .ThenInclude(se => se.SalonInformation),
                        orderBy: query => query.OrderBy(a => a.AppointmentDetails!
                            .OrderByDescending(ad => ad.StartTime)!
                            .FirstOrDefault()!.StartTime),
                        page: page,
                        size: size
                    );
            }
            else
            {
                appointments = await _unitOfWork.GetRepository<Appointment>()
                    .GetPagingListAsync(
                        predicate: predicate,
                        include: query => query.Include(a => a.Customer)
                                               .Include(a => a.AppointmentDetails)
                                                   .ThenInclude(ad => ad.SalonEmployee)
                                                       .ThenInclude(se => se.SalonInformation),
                        orderBy: query => query.OrderByDescending(a => a.AppointmentDetails!
                            .OrderByDescending(ad => ad.StartTime)!
                            .FirstOrDefault()!.StartTime),
                        page: page,
                        size: size
                    );
            }

            // Xắp sếp appointment detail tăng dần
            foreach (var appointment in appointments.Items)
            {
                appointment.AppointmentDetails = appointment.AppointmentDetails
                    .OrderBy(ad => ad.StartTime)
                    .ToList();
            }

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
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
                }
            }
            return appointmentResponse!;
        }

        public async Task<List<GetAppointmentResponse>> GetAppointmentSalonByStatusNoPaing(Guid salonId, string? status, DateTime? startDate, DateTime? endDate)
        {

            var predicate = PredicateBuilder.New<Appointment>(x =>
        x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId) &&
        (string.IsNullOrEmpty(status) || x.Status == status) &&
        x.StartDate >= startDate.Value &&
        x.StartDate <= endDate.Value
    );
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetListAsync(
                    predicate: predicate,
                    include: query => query.Include(a => a.Customer)
                                           .Include(a => a.AppointmentDetails)
                                               .ThenInclude(ad => ad.SalonEmployee)
                                                   .ThenInclude(se => se.SalonInformation)
                );
            var appointmentResponse = _mapper.Map<List<GetAppointmentResponse>>(appointments);

            if (appointmentResponse != null && appointmentResponse.Count > 0)
            {
                foreach (var item in appointmentResponse)
                {
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
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

            foreach (var item in appointmentResponse.Items)
            {
                item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
            }

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
            DateTime dateTime = DateTime.Now;
            TimeOnly timeOnlyNow = TimeOnly.FromDateTime(dateTime);
            decimal timeStartSchedule = salonSchedule.StartTime.Hour + (decimal)salonSchedule.StartTime.Minute / 60;
            decimal timeEndSchedule = salonSchedule.EndTime.Hour + (decimal)salonSchedule.EndTime.Minute / 60;

            if (salonSchedule.DayOfWeek.Equals(DateTime.Now.DayOfWeek.ToString()))
            {
                // Now between start and end schedule of salon schedule
                if (salonSchedule.StartTime <= timeOnlyNow && salonSchedule.EndTime >= timeOnlyNow)
                {
                    bool checkTime = true;
                    decimal start = salonSchedule.StartTime.Hour + salonSchedule.StartTime.Minute / 60m;
                    decimal end = salonSchedule.EndTime.Hour + salonSchedule.EndTime.Minute / 60m;
                    for (decimal i = start; i <= end; i += 0.25m)
                        if (timeOnlyNow.Hour + timeOnlyNow.Minute / 60m <= i)
                        {
                            timeStartSchedule = i + 1;
                            checkTime = false;
                            break;
                        }
                    if (timeStartSchedule > timeEndSchedule || checkTime)
                    {
                        return result;
                    }
                }

                if (timeOnlyNow >= salonSchedule.EndTime)
                {
                    return result;
                }
            }

            List<decimal> availbeTimeResponse = GenerateTimeSlot(timeStartSchedule, timeEndSchedule, 0.25m);
            var availableTimesDict = availbeTimeResponse.ToDictionary(timeSlot => timeSlot, timeSlot => new List<EmployeeAvailable>());
            // Loại bỏ các time slots đã qua
            //decimal currentTime = DateTime.Now.Hour + (decimal)DateTime.Now.Minute / 60;
            //availableTimesDict = availableTimesDict
            //    .Where(kvp => kvp.Key > currentTime)
            //    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            //if (availableTimesDict.Count == 0)
            //{
            //    throw new NotFoundException("Đã qua giờ làm việc của salon, barber shop");
            //}

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
                //Xủ lý khi chọn employee nào cũng được => IsAnyOne = true
                var serviceHair = await _unitOfWork.GetRepository<ServiceHair>()
                                                           .SingleOrDefaultAsync(predicate: x => x.Id == request.ServiceHairId && x.IsActive);
                var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync
                                                  (
                                                    predicate: x => x.SalonInformationId == request.SalonId &&
                                                                    x.ServiceEmployees.Any(se => se.ServiceHairId == request.ServiceHairId)
                                                                    && x.IsActive == true
                                                  );
                if (employees == null)
                {
                    throw new NotFoundException("Không tìm thấy nhân viên của salon, barber shop có thể phục vụ dịch vụ này");
                }

                if (employees == null)
                {
                    throw new NotFoundException("Salon hiện không có nhân viên làm việc");
                }

                var tempAvailableTimes = new Dictionary<decimal, List<EmployeeAvailable>>();

                foreach (var employee in employees)
                //if (employee.Id.ToString().Equals("3644a197-4c84-4e6d-a4b1-5e9c82363d25"))
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
                    // Get time work of employee
                    var startSchedule = scheduleEmp.StartTime.Hour + (decimal)scheduleEmp.StartTime.Minute / 60;
                    var endSchedule = scheduleEmp.EndTime.Hour + (decimal)scheduleEmp.EndTime.Minute / 60;

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

                    int i = 0;
                    while (timeSlotEmployee.Count > 0 && i < timeSlotEmployee.Count && timeSlotEmployee[i] + serviceHair.Time > timeEndSchedule)
                    {
                        if (i + 1 < timeSlotEmployee.Count && timeSlotEmployee[i] + 0.25m != timeSlotEmployee[i + 1] && 0.25m <= serviceHair.Time)
                        {
                            timeSlotEmployee.Remove(timeSlotEmployee[i]);
                        }
                        else
                        {
                            bool check = false;
                            for (int j = i + 1; j < timeSlotEmployee.Count; j++)
                            {
                                if (((j + 1 < timeSlotEmployee.Count && timeSlotEmployee[j] + 0.25m == timeSlotEmployee[j + 1]) || j == timeSlotEmployee.Count - 1)
                                        && timeSlotEmployee[j] - timeSlotEmployee[i] + 0.25m >= serviceHair.Time)
                                {
                                    i++;
                                    break;
                                }
                                else if (j + 1 < timeSlotEmployee.Count && timeSlotEmployee[j] + 0.25m != timeSlotEmployee[j + 1] && timeSlotEmployee[j] - timeSlotEmployee[i] + 0.25m < serviceHair.Time)
                                {
                                    timeSlotEmployee.RemoveAll(slot => slot >= timeSlotEmployee[i] && slot <= timeSlotEmployee[j]);
                                    break;
                                }
                            }
                        }
                    }

                    timeSlotEmployee.RemoveAll(slot => slot > timeEndSchedule - serviceHair.Time && slot <= timeEndSchedule);

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
                        throw new NotFoundException($"Không tìm thấy nhân viên phục vụ cho dịch vụ - {serviceHair.ServiceName} vào {(int)request.AvailableSlot}:{(int)((request.AvailableSlot - (int)request.AvailableSlot) * 60)}");
                    }
                    else
                    {
                        throw new NotFoundException($"Không tìm thấy nhân viên phục vụ cho dịch vụ - {serviceHair.ServiceName}");
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
            var config = await _unitOfWork.GetRepository<Config>().SingleOrDefaultAsync(predicate: x => x.CommissionRate != null && x.IsActive);
            if (config == null)
            {
                throw new NotFoundException("Không tìm thấy config phần trăm hoa hồng");
            }

            //Kiểm tra lịch làm việc employee
            foreach (var appointmentItem in request.AppointmentDetails)
            {
                var appointmentDetailDomain = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                            .SingleOrDefaultAsync(
                                                                predicate: x => ((x.StartTime >= appointmentItem.StartTime && x.StartTime < appointmentItem.EndTime) ||
                                                                                (x.EndTime > appointmentItem.StartTime && x.EndTime <= appointmentItem.EndTime) ||
                                                                                (x.StartTime <= appointmentItem.StartTime && x.EndTime >= appointmentItem.EndTime))
                                                                                && x.SalonEmployeeId == appointmentItem.SalonEmployeeId
                                                            );
                if (appointmentDetailDomain != null)
                {
                    throw new Exception("Thời gian vừa có khách hàng đặt. Hãy đặt lại ở khung giờ khác nhé");
                }
            }

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
                CommissionRate = config.CommissionRate,
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
                                                                          .Include(s => s.Customer)
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
            appoinment.CancelDate = DateTime.Now;
            //Delete QR image
            await _mediaService.DeleteImageAsync(appoinment!.QrCodeImg!, MediaPath.QR_APPOINTMENT);
            appoinment.QrCodeImg = "";
            _unitOfWork.GetRepository<Appointment>().UpdateAsync(appoinment);

            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            if (isUpdate)
            {
                //Send mail notification
                DateTime TimeBook = appoinment.AppointmentDetails.OrderBy(s => s.StartTime).FirstOrDefault().StartTime;
                SalonInformation Salon = appoinment.AppointmentDetails.FirstOrDefault().SalonEmployee.SalonInformation;
                string bodyEmail = $"Chúng tôi rất tiếc phải thông báo rằng khách hàng {appoinment.Customer.FullName} của bạn đã hủy lịch hẹn cắt tóc có thời gian vào {TimeBook.Hour}:{TimeBook.Minute} ngày {appoinment.StartDate.Day}, tháng {appoinment.StartDate.Month}, năm {appoinment.StartDate.Year}. Lý do: {cancelApointmentRequest.reasonCancel}. Vui lòng kiểm tra lại lịch trình của bạn để biết thêm chi tiết";
                await _emailService.SendEmailWithBodyAsync(Salon.SalonOwner.Email, "Thông báo hủy đơn đặt lịch trên Hairhub", Salon.Name, bodyEmail);
            }
            return isUpdate;
        }

        public async Task<DataOfMonths> GetAppointmentbyStatusByAdmin(string status, int year)
        {
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            var appointments = await _unitOfWork.GetRepository<Appointment>().GetListAsync(predicate: p => p.Status == status && p.StartDate.Year == year);
            var dataOfMonths = new DataOfMonths
            {
                Jan = appointments.Count(a => a.StartDate.Month == 1),
                Feb = appointments.Count(a => a.StartDate.Month == 2),
                March = appointments.Count(a => a.StartDate.Month == 3),
                April = appointments.Count(a => a.StartDate.Month == 4),
                May = appointments.Count(a => a.StartDate.Month == 5),
                June = appointments.Count(a => a.StartDate.Month == 6),
                July = appointments.Count(a => a.StartDate.Month == 7),
                August = appointments.Count(a => a.StartDate.Month == 8),
                September = appointments.Count(a => a.StartDate.Month == 9),
                October = appointments.Count(a => a.StartDate.Month == 10),
                November = appointments.Count(a => a.StartDate.Month == 11),
                December = appointments.Count(a => a.StartDate.Month == 12)
            };
            return dataOfMonths;

        }

        public async Task<DataOfMonths> GetRevenueByAdmin(int year)
        {
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            var payments = await _unitOfWork.GetRepository<Appointment>().GetListAsync(predicate: p => p.StartDate.Year == year && p.Status == AppointmentStatus.Successed);
            var dataOfMonths = new DataOfMonths
            {
                Jan = (int?)payments.Where(a => a.StartDate.Month == 1).Sum(a => a.TotalPrice),
                Feb = (int?)payments.Where(a => a.StartDate.Month == 2).Sum(a => a.TotalPrice),
                March = (int?)payments.Where(a => a.StartDate.Month == 3).Sum(a => a.TotalPrice),
                April = (int?)payments.Where(a => a.StartDate.Month == 4).Sum(a => a.TotalPrice),
                May = (int?)payments.Where(a => a.StartDate.Month == 5).Sum(a => a.TotalPrice),
                June = (int?)payments.Where(a => a.StartDate.Month == 6).Sum(a => a.TotalPrice),
                July = (int?)payments.Where(a => a.StartDate.Month == 7).Sum(a => a.TotalPrice),
                August = (int?)payments.Where(a => a.StartDate.Month == 8).Sum(a => a.TotalPrice),
                September = (int?)payments.Where(a => a.StartDate.Month == 9).Sum(a => a.TotalPrice),
                October = (int?)payments.Where(a => a.StartDate.Month == 10).Sum(a => a.TotalPrice),
                November = (int?)payments.Where(a => a.StartDate.Month == 11).Sum(a => a.TotalPrice),
                December = (int?)payments.Where(a => a.StartDate.Month == 12).Sum(a => a.TotalPrice)
            };
            return dataOfMonths;
        }

        public async Task<DataOfMonths> GetCommissionByAdmin(int year)
        {
            if (year == 0)
            {
                year = DateTime.Now.Year;
            }
            var payments = await _unitOfWork.GetRepository<Payment>().GetListAsync(predicate: p => p.PaymentDate.Year == year && p.Status == PaymentStatus.Paid);
            var dataOfMonths = new DataOfMonths
            {
                Jan = (int?)payments.Where(a => a.PaymentDate.Month == 1).Sum(a => a.TotalAmount),
                Feb = (int?)payments.Where(a => a.PaymentDate.Month == 2).Sum(a => a.TotalAmount),
                March = (int?)payments.Where(a => a.PaymentDate.Month == 3).Sum(a => a.TotalAmount),
                April = (int?)payments.Where(a => a.PaymentDate.Month == 4).Sum(a => a.TotalAmount),
                May = (int?)payments.Where(a => a.PaymentDate.Month == 5).Sum(a => a.TotalAmount),
                June = (int?)payments.Where(a => a.PaymentDate.Month == 6).Sum(a => a.TotalAmount),
                July = (int?)payments.Where(a => a.PaymentDate.Month == 7).Sum(a => a.TotalAmount),
                August = (int?)payments.Where(a => a.PaymentDate.Month == 8).Sum(a => a.TotalAmount),
                September = (int?)payments.Where(a => a.PaymentDate.Month == 9).Sum(a => a.TotalAmount),
                October = (int?)payments.Where(a => a.PaymentDate.Month == 10).Sum(a => a.TotalAmount),
                November = (int?)payments.Where(a => a.PaymentDate.Month == 11).Sum(a => a.TotalAmount),
                December = (int?)payments.Where(a => a.PaymentDate.Month == 12).Sum(a => a.TotalAmount)
            };
            return dataOfMonths;
        }

        public async Task<List<RatioData>> GetPercentagebyStatusOfAppointmentByAdmin(int? year)
        {
            if (year == 0 || year == null)
            {
                year = DateTime.Now.Year;
            }

            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetListAsync(predicate: p => p.StartDate.Year == year &&
                                               (p.Status == AppointmentStatus.CancelByCustomer ||
                                                p.Status == AppointmentStatus.Fail ||
                                                p.Status == AppointmentStatus.Successed ||
                                                p.Status == AppointmentStatus.Booking));

            var totalAppointments = appointments.Count;


            var ratioData = new List<RatioData>
            {
                new RatioData { Status = "Thành công", Percentage = 0 },
                new RatioData { Status = "Thất bại", Percentage = 0 },
                new RatioData { Status = "Hủy bởi khách hàng", Percentage = 0 },
            };

            if (totalAppointments == 0)
            {
                return ratioData;
            }


            var groupedData = appointments
                .GroupBy(a => a.Status)
                .Select(g => new RatioData
                {
                    Status = GetStatusLabel(g.Key),
                    Percentage = (double)g.Count() / totalAppointments * 100
                })
                .ToList();

            foreach (var data in groupedData)
            {
                var ratio = ratioData.FirstOrDefault(r => r.Status == data.Status);
                if (ratio != null)
                {
                    ratio.Percentage = data.Percentage;
                }
            }

            return ratioData;
        }

        private string GetStatusLabel(string status)
        {
            return status switch
            {
                var s when s == AppointmentStatus.Successed => "Thành công",
                var s when s == AppointmentStatus.Fail => "Thất bại",
                var s when s == AppointmentStatus.CancelByCustomer => "Hủy bởi khách hàng",
                _ => "Khác"
            };
        }

        public async Task<List<MonthlyRatioData>> GetPercentageOfAppointmentByAdmin(int? year)
        {
            var currentYear = DateTime.Now.Year;
            if (year == 0)
            {
                year = currentYear;
            }

            var monthlyData = new List<MonthlyRatioData>();

            for (int month = 1; month <= 12; month++)
            {
                var appointments = await _unitOfWork.GetRepository<Appointment>()
                    .GetListAsync(predicate: p => p.StartDate.Year == year && p.StartDate.Month == month &&
                                                   (p.Status == AppointmentStatus.CancelByCustomer ||
                                                    p.Status == AppointmentStatus.Fail ||
                                                    p.Status == AppointmentStatus.Successed));

                var totalAppointments = appointments.Count;
                if (totalAppointments == 0)
                {
                    monthlyData.Add(new MonthlyRatioData
                    {
                        Month = "Tháng " + month,
                        Success = 0,
                        Failed = 0,
                        Canceled = 0
                    });
                    continue;
                }
                var groupedAppointments = appointments
                    .GroupBy(a => a.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Percentage = (double)g.Count() / totalAppointments * 100
                    })
                    .ToList();

                var successPercentage = groupedAppointments
                    .FirstOrDefault(g => g.Status == AppointmentStatus.Successed)?.Percentage ?? 0;
                var failedPercentage = groupedAppointments
                    .FirstOrDefault(g => g.Status == AppointmentStatus.Fail)?.Percentage ?? 0;
                var canceledPercentage = groupedAppointments
                    .FirstOrDefault(g => g.Status == AppointmentStatus.CancelByCustomer)?.Percentage ?? 0;

                monthlyData.Add(new MonthlyRatioData
                {
                    Month = "Tháng " + month,
                    Success = successPercentage,
                    Failed = failedPercentage,
                    Canceled = canceledPercentage
                });
            }

            return monthlyData;
        }

        public async Task<IPaginate<GetAppointmentResponse>> GetAppointmentByCustomerIdStatus(int page, int size, Guid customerId, string? status)
        {
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetPagingListAsync(
                    predicate: x => x.CustomerId == customerId && x.Status.Contains(status ?? ""),
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
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
                }
            }
            return appointmentResponse;
        }

        //25cb20ef-6cb2-4db9-aa19-8f5c918ee6dd
        public async Task<IPaginate<GetAppointmentResponse>> GetAppointmentCustomerByStatus(Guid customerId, string? status, bool isAscending, DateTime? date, int page, int size)
        {
            status = string.IsNullOrEmpty(status) ? "" : status;
            ExpressionStarter<Appointment> predicate;
            if (date.HasValue)
            {
                predicate = PredicateBuilder.New<Appointment>(x => x.CustomerId == customerId && x.Status.Contains(status) && x.StartDate.Date == date.Value.Date);

            }
            else
            {
                predicate = PredicateBuilder.New<Appointment>(x => x.CustomerId == customerId && x.Status.Contains(status));
            }
            IPaginate<Appointment> appointments;
            if (isAscending)
            {
                appointments = await _unitOfWork.GetRepository<Appointment>()
                                                .GetPagingListAsync(
                                                    predicate: predicate,
                                                    include: query => query.Include(a => a.Customer)
                                                                           .Include(a => a.AppointmentDetails)
                                                                               .ThenInclude(ad => ad.SalonEmployee)
                                                                                   .ThenInclude(se => se.SalonInformation),
                                                    orderBy: query => query.OrderBy(a => a.AppointmentDetails!
                                                                            .OrderByDescending(ad => ad.StartTime)!
                                                                            .FirstOrDefault()!.StartTime),
                                                    page: page,
                                                    size: size
                                                );
            }
            else
            {
                appointments = await _unitOfWork.GetRepository<Appointment>()
                                .GetPagingListAsync(
                                    predicate: predicate,
                                    include: query => query.Include(a => a.Customer)
                                                           .Include(a => a.AppointmentDetails)
                                                               .ThenInclude(ad => ad.SalonEmployee)
                                                                   .ThenInclude(se => se.SalonInformation),
                                    orderBy: query => query.OrderByDescending(a => a.AppointmentDetails!
                                                            .OrderByDescending(ad => ad.StartTime)!
                                                            .FirstOrDefault()!.StartTime),
                                    page: page,
                                    size: size
                                );
            }
            // Xắp sếp appointment detail tăng dần
            foreach (var appointment in appointments.Items)
            {
                appointment.AppointmentDetails = appointment.AppointmentDetails
                    .OrderBy(ad => ad.StartTime)
                    .ToList();
            }

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
                    item.IsFeedback = await _unitOfWork.GetRepository<Feedback>().SingleOrDefaultAsync(predicate: x => x.AppointmentId == item.Id && x.IsActive == true) != null;
                }
            }
            return appointmentResponse;
        }
        #endregion

    }
}
