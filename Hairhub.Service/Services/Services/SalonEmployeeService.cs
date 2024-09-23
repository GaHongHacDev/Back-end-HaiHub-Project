using AutoMapper;
using Hairhub.Common.Security;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Requests.Appointments;
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
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using static QRCoder.PayloadGenerator;

namespace Hairhub.Service.Services.Services
{
    public class SalonEmployeeService : ISalonEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IScheduleService _scheduleService;
        private readonly IEmailService _emailService;

        public SalonEmployeeService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService, IScheduleService scheduleService, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
            _scheduleService = scheduleService;
            _emailService = emailService;
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

        public async Task<bool> CreateAccountEmployee(CreateAccountEmployeeRequest request)
        {
            request.Email = request.Email.Trim();
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.UserName.Equals(request.Email) && x.IsActive);
            if (account != null)
            {
                throw new NotFoundException("Email đã tồn tại");
            }
            var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == request.EmployeeId);
            if (employee.AccountId != null) 
            {
                throw new NotFoundException("Nhân viên đã có tài khoản");
            }
            Account accountEmployee = new Account();
            accountEmployee.UserName = request.Email;
            accountEmployee.Password = AesEncoding.GenerateRandomPassword();
            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleName.Equals(RoleEnum.SalonEmployee.ToString()));
            accountEmployee.RoleId = role.RoleId;
            accountEmployee.IsActive = true;
            accountEmployee.CreatedDate = DateTime.Now;
            accountEmployee.Id = Guid.NewGuid();
            
            employee.Email = request.Email;
            employee.AccountId = accountEmployee.Id;
            _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(employee);
            await _unitOfWork.GetRepository<Account>().InsertAsync(accountEmployee);
            bool isSuccess = await _unitOfWork.CommitAsync()>0;
            if (isSuccess) 
            {
                await _emailService.SendEmailRegisterAccountAsync(request.Email, "Tạo tài khoản thành công trên Hairhub", employee.FullName, accountEmployee.UserName, accountEmployee.Password);
            }
            return isSuccess;
        }

        public async Task<bool> CreateSalonEmployee(CreateSalonEmployeeRequest request)
        {
            //check exist salon
            var existSalon = await _unitOfWork.GetRepository<SalonInformation>()
                                        .SingleOrDefaultAsync(predicate: x => x.Id == request.SalonInformationId);
            if (existSalon == null)
            {
                throw new NotFoundException($"Not found salon with id {request.SalonInformationId}");
            }
            var scheduleSalon = await _unitOfWork.GetRepository<Schedule>().GetListAsync(predicate: x => x.SalonId == request.SalonInformationId);
            //Check schedule employee and salon
            foreach (var emp in request.SalonEmployees)
            {
                foreach (var scheduleEmp in emp.ScheduleEmployees)
                {
                    var salonDayOfWeek = scheduleSalon.FirstOrDefault(s => s.DayOfWeek.ToLower().Equals(scheduleEmp.Date!.ToLower()));
                    if (!salonDayOfWeek!.IsActive && scheduleEmp.IsActive)
                    {
                        throw new Exception($"Salon, barber shop không có lịch làm việc vào {scheduleEmp.Date}");
                    }
                    if (scheduleEmp.StartTime < salonDayOfWeek!.StartTime)
                    {
                        throw new Exception($"Salon, barber shop bắt đầu làm việc từ {salonDayOfWeek.StartTime}");
                    }
                    else if (scheduleEmp.EndTime > salonDayOfWeek!.EndTime)
                    {
                        throw new Exception($"Salon, barber shop kết thúc giờ làm việc vào {salonDayOfWeek.EndTime}");
                    }
                }

            }

            //create employee
            foreach (var item in request.SalonEmployees)
            {
                var employee = _mapper.Map<SalonEmployee>(item);
                employee.Id = Guid.NewGuid();
                var url = await _mediaService.UploadAnImage(item.ImgEmployee, MediaPath.EMPLOYEE, employee.Id.ToString());
                employee.Img = url;
                employee.SalonInformationId = request.SalonInformationId;
                employee.Rating = 0;
                employee.RatingSum = 0;
                employee.RatingCount = 0;
                await _unitOfWork.GetRepository<SalonEmployee>().InsertAsync(employee);
                if (item.ScheduleEmployees == null || item.ScheduleEmployees.Count == 0)
                {
                    throw new NotFoundException("Không tìm thấy lịch làm việc của nhân viên");
                }
                //create schedule
                foreach (var itemSchedule in item.ScheduleEmployees)
                {
                    var scheduleEmployee = new CreateScheduleRequest()
                    {
                        EmployeeId = employee.Id,
                        DayOfWeek = itemSchedule.Date,
                        EndTime = itemSchedule.EndTime,
                        StartTime = itemSchedule.StartTime,
                        IsActive = itemSchedule.IsActive
                    };
                    _scheduleService.CreateScheduleEmployee(scheduleEmployee);
                }
                //create Service Employee
                foreach (var itemServiceHair in item.ServiceHairId)
                {
                    ServiceEmployee srvEmployee = new ServiceEmployee();
                    srvEmployee.Id = Guid.NewGuid();
                    srvEmployee.ServiceHairId = itemServiceHair;
                    srvEmployee.SalonEmployeeId = employee.Id;
                    await _unitOfWork.GetRepository<ServiceEmployee>().InsertAsync(srvEmployee);
                }
            }
            if (existSalon.Status != SalonStatus.Approved)
            {
                existSalon.Status = SalonStatus.Pending;
                _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(existSalon);
            }
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            return isInsert;
        }

        public async Task<bool> DeleteSalonEmployeeById(Guid id)
        {
            try
            {
                var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                var existingappointmentdetail = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                                predicate: p => p.SalonEmployeeId == salonEmployee.Id
                                                && p.Appointment.StartDate >= DateTime.Now.Date && p.Appointment.Status == AppointmentStatus.Booking
                                                , include: i => i.Include(o => o.Appointment)
                                                );
                if (salonEmployee == null)
                {
                    throw new NotFoundException("Không tìm thấy nhân viên này");
                }

                if (existingappointmentdetail == null || existingappointmentdetail.Count == 0)
                {
                    salonEmployee.IsActive = false;
                    _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(salonEmployee);
                    bool isUpdate = await _unitOfWork.CommitAsync() > 0;
                    return isUpdate;
                }
                else
                {
                    throw new Exception("Không thể xóa nhân viên này vì đã có khách hàng đặt lịch");
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

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

        public async Task<GetSalonEmployeeResponse?> GetSalonEmployeeById(Guid id)
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

        public async Task<IPaginate<GetSalonEmployeeResponse>> GetSalonEmployeeBySalonInformationId(Guid SalonInformationId, int page, int size, bool? orderName,
                                                                                                    bool? isActive, string? nameEmployee)
        {
            Expression<Func<SalonEmployee, bool>> predicate = x => x.SalonInformationId.Equals(SalonInformationId);

            if (isActive.HasValue)
            {
                predicate = predicate.And(x => x.IsActive == isActive.Value);
            }

            if (!string.IsNullOrEmpty(nameEmployee))
            {
                predicate = predicate.And(x => x.FullName.Contains(nameEmployee));
            }

            Func<IQueryable<SalonEmployee>, IOrderedQueryable<SalonEmployee>> orderBy = null;
            if (orderName.HasValue)
            {
                orderBy = orderName.Value
                    ? (Func<IQueryable<SalonEmployee>, IOrderedQueryable<SalonEmployee>>)(q => q.OrderBy(x => x.FullName))
                    : (Func<IQueryable<SalonEmployee>, IOrderedQueryable<SalonEmployee>>)(q => q.OrderByDescending(x => x.FullName));
            }

            var salonEmployees = await _unitOfWork.GetRepository<SalonEmployee>()
                                                  .GetPagingListAsync(
                                                      predicate: predicate,
                                                      orderBy: orderBy,
                                                      include: query => query.Include(s => s.Schedules)
                                                                             .Include(s => s.ServiceEmployees)
                                                                             .ThenInclude(se => se.ServiceHair),
                                                      page: page,
                                                      size: size
                                                  );

            if (salonEmployees == null)
                return null;

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

        public async Task<bool> UpdateSalonEmployeeById(Guid id, UpdateSalonEmployeeRequest updateSalonEmployeeRequest)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == id);

            if (salonEmployee == null)
            {
                throw new NotFoundException("Không tìm thấy nhân viên salon!");
            }

            if (!string.IsNullOrEmpty(updateSalonEmployeeRequest.FullName))
            {
                salonEmployee.FullName = updateSalonEmployeeRequest.FullName;
            }

            if (updateSalonEmployeeRequest.Gender != null)
            {
                salonEmployee.Gender = updateSalonEmployeeRequest.Gender;
            }

            if (!string.IsNullOrEmpty(updateSalonEmployeeRequest.Phone))
            {
                salonEmployee.Phone = updateSalonEmployeeRequest.Phone;
            }

            if (updateSalonEmployeeRequest.Img != null)
            {
                var url = await _mediaService.UploadAnImage(updateSalonEmployeeRequest.Img, MediaPath.EMPLOYEE, salonEmployee.Id.ToString());
                salonEmployee.Img = url;
            }

            salonEmployee.IsActive = updateSalonEmployeeRequest.IsActive;

            _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(salonEmployee);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<IList<GetEmployeeHighRatingResponse>> GetEmployeeHighRating(int? numberOfDay)
        {
            var employees = await _unitOfWork.GetRepository<SalonEmployee>()
                                            .GetListAsync(
                                                          predicate: x => x.IsActive && x.SalonInformation.Status.Equals(SalonStatus.Approved) && x.Rating!=0,
                                                            orderBy: q => q.OrderByDescending(s => s.Rating)
                                                                            .ThenByDescending(s => s.RatingCount),
                                                            take: 10
                                                         );
            if (employees==null)
            {
                employees = new List<SalonEmployee>();
            }
            return _mapper.Map<IList<GetEmployeeHighRatingResponse>>(employees);
        }
    }
}
