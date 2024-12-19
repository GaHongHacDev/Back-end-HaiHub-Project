using AutoMapper;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Common.ThirdParties.Implementation;
using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.SalonInformations;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Helpers;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing.Printing;
using System.Linq;
using static QRCoder.PayloadGenerator;

namespace Hairhub.Service.Services.Services
{
    public class SalonInformationService : ISalonInformationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;
        private readonly IScheduleService _scheduleService;

        public SalonInformationService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService, IScheduleService scheduleService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
            _scheduleService = scheduleService;
        }

        public async Task<bool> ActiveSalonInformation(Guid id)
        {
            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonInformation == null)
            {
                throw new NotFoundException("SalonInformation not found!");
            }
            salonInformation.Status = SalonStatus.Approved;
            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<CreateSalonInformationResponse> CreateSalonInformation(CreateSalonInformationRequest createSalonInformationRequest)
        {
            var owner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createSalonInformationRequest.OwnerId));
            if (owner == null)
            {
                throw new NotFoundException("OwnerId not found");
            }
            var salonInformation = _mapper.Map<SalonInformation>(createSalonInformationRequest);
            salonInformation.Id = Guid.NewGuid();
            salonInformation.Status = SalonStatus.Creating;
            string url = await _mediaService.UploadAnImage(createSalonInformationRequest.Img, MediaPath.SALON_AVATAR, salonInformation.Id.ToString());
            salonInformation.Img = url;
            salonInformation.Rate = 0;
            salonInformation.TotalRating = 0;
            salonInformation.TotalReviewer = 0;
            salonInformation.NumberOfReported = 0;
            salonInformation.CreatedAt = DateTime.Now;
            await _unitOfWork.GetRepository<SalonInformation>().InsertAsync(salonInformation);
            foreach (var scheduleRequest in createSalonInformationRequest.SalonInformationSchedules)
            {
                var newSchedule = new Schedule
                {
                    Id = Guid.NewGuid(),
                    DayOfWeek = scheduleRequest.DayOfWeek,
                    SalonId = salonInformation.Id,
                    StartTime = TimeOnly.Parse(scheduleRequest.StartTime),
                    EndTime = TimeOnly.Parse(scheduleRequest.EndTime),
                    IsActive = scheduleRequest.IsActive
                };
                await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);
            }

            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            if (!isInsert)
            {
                throw new Exception("Không thể tạo salon");
            }
            return _mapper.Map<CreateSalonInformationResponse>(salonInformation);
        }

        public async Task<bool> DeleteSalonInformationById(Guid id)
        {
            try
            {
                var salonInformation = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(predicate: p => p.SalonInformationId == id);
                if (employees == null || !employees.Any())
                {
                    throw new NotFoundException("Không tìm thấy nhân viên nào trong salon này");
                }

                foreach (var employee in employees)
                {
                    var existingAppointments = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                               predicate: a => a.SalonEmployeeId == employee.Id
                                               && a.Status == AppointmentStatus.Booking && a.StartTime > DateTime.UtcNow.Date);
                    if (existingAppointments.Any())
                    {
                        throw new Exception("Không thể xóa salon này vì có nhân viên có lịch hẹn");
                    }
                }
                salonInformation.Status = SalonStatus.Disable;
                _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);
                bool isUpdate = await _unitOfWork.CommitAsync() > 0;
                return isUpdate;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IPaginate<GetSalonInformationResponse>> GetAllApprovedSalonInformation(int page, int size)
        {
            var salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
           .GetPagingListAsync(
                predicate: x => x.Status.Equals(SalonStatus.Approved),
               include: query => query.Include(s => s.SalonOwner).Include(x => x.Schedules),
               page: page,
               size: size
           );

            var salonInformationResponses = new Paginate<GetSalonInformationResponse>()
            {
                Page = salonInformations.Page,
                Size = salonInformations.Size,
                Total = salonInformations.Total,
                TotalPages = salonInformations.TotalPages,
                Items = _mapper.Map<IList<GetSalonInformationResponse>>(salonInformations.Items),
            };
            return salonInformationResponses;
        }

        public async Task<List<GetSalonInformationResponse>> GetAllApprovedSalonInformationNoPaging()
        {
            var salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
           .GetListAsync(
                predicate: x => x.Status.Equals(SalonStatus.Approved),
               include: query => query.Include(s => s.SalonOwner).Include(x => x.Schedules)
           );

            var salonInformationResponses = _mapper.Map<List<GetSalonInformationResponse>>(salonInformations);
            return salonInformationResponses;
        }

        public async Task<IPaginate<GetSalonInformationResponse>> GetAllSalonByAdmin(int page, int size)
        {
            var salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.SalonOwner).Include(x => x.Schedules),
               page: page,
               size: size
           );

            var salonInformationResponses = new Paginate<GetSalonInformationResponse>()
            {
                Page = salonInformations.Page,
                Size = salonInformations.Size,
                Total = salonInformations.Total,
                TotalPages = salonInformations.TotalPages,
                Items = _mapper.Map<IList<GetSalonInformationResponse>>(salonInformations.Items),
            };
            return salonInformationResponses;
        }

        public async Task<IPaginate<GetSalonInformationResponse>> GetSalonByStatus(string? name, string? status, int page, int size)
        {


            var salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
        .GetPagingListAsync(
            predicate: c =>
            (string.IsNullOrEmpty(name) || c.Name.Contains(name)) &&
                (string.IsNullOrEmpty(status) || c.Status == status),
            include: query => query.Include(s => s.SalonOwner),
            page: page,
            size: size
        );

            var salonInformationResponses = new Paginate<GetSalonInformationResponse>()
            {
                Page = salonInformations.Page,
                Size = salonInformations.Size,
                Total = salonInformations.Total,
                TotalPages = salonInformations.TotalPages,
                Items = _mapper.Map<IList<GetSalonInformationResponse>>(salonInformations.Items),
            };
            return salonInformationResponses;
        }

        public async Task<GetSalonInformationResponse>? GetSalonInformationById(Guid id)
        {
            SalonInformation salonInformationResponse = await _unitOfWork
                .GetRepository<SalonInformation>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: source => source.Include(s => s.SalonOwner).Include(x => x.Schedules)
                 );
            if (salonInformationResponse == null)
                return null;
            return _mapper.Map<GetSalonInformationResponse>(salonInformationResponse);
        }

        public async Task<GetSalonInformationResponse>? GetSalonByOwnerId(Guid ownerId)
        {
            SalonInformation salonInformation = await _unitOfWork
                .GetRepository<SalonInformation>()
                .SingleOrDefaultAsync(
                    predicate: x => x.OwnerId == ownerId,
                    include: source => source.Include(s => s.SalonOwner)
                 );
            if (salonInformation == null)
                throw new NotFoundException($"Not found salon information with owner id {ownerId}");
            var salonInforResponse = _mapper.Map<GetSalonInformationResponse>(salonInformation);
            var schedules = await _scheduleService.GetSalonSchedules(salonInformation.Id);
            var orderedSchedules = schedules
                .OrderBy(s => (int)Enum.Parse<DayOfWeek>(s.DayOfWeek) == 0 ? 7 : (int)Enum.Parse<DayOfWeek>(s.DayOfWeek))
                .ToList();
            if (schedules.Any())
            {
                salonInforResponse.schedules = schedules;
            }
            return salonInforResponse;
        }

        public async Task<bool> UpdateSalonInformationById(Guid id, UpdateSalonInformationRequest updateSalonInformationRequest)
        {
            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonInformation == null)
            {
                throw new NotFoundException("SalonInformation not found!");
            }
            if (!string.IsNullOrEmpty(updateSalonInformationRequest.Name))
            {
                salonInformation.Name = updateSalonInformationRequest.Name;
            }

            if (!string.IsNullOrEmpty(updateSalonInformationRequest.Address))
            {
                salonInformation.Address = updateSalonInformationRequest.Address;
            }

            if (!string.IsNullOrEmpty(updateSalonInformationRequest.Description))
            {
                salonInformation.Description = updateSalonInformationRequest.Description;
            }

            if (updateSalonInformationRequest.Image != null)
            {

                salonInformation.Img = await _mediaService.UploadAnImage(updateSalonInformationRequest.Image, MediaPath.SALON_AVATAR, salonInformation.Id.ToString());
            }

            if (updateSalonInformationRequest.Longitude != null)
            {
                salonInformation.Longitude = (decimal)updateSalonInformationRequest.Longitude;
            }

            if (updateSalonInformationRequest.Latitude != null)
            {
                salonInformation.Latitude = (decimal)updateSalonInformationRequest.Latitude;
            }
            salonInformation.UpdatedAt = DateTime.Now;
            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<IPaginate<SearchSalonByNameAddressServiceResponse>> SearchSalonByNameAddressService(int page, int size, string? serviceName = "",
                                        string? salonAddress = "", string? salonName = "", decimal? latitude = 0, decimal? longtitude = 0, decimal? distance = 0)
        {
            serviceName = serviceName?.Trim() ?? string.Empty;
            salonAddress = salonAddress?.Trim() ?? string.Empty;
            salonName = salonName?.Trim() ?? string.Empty;
            IPaginate<SalonInformation> salonInformations;
            if (latitude != null && longtitude != null && distance != null)
            {
                var salonQuery = await _unitOfWork.GetRepository<SalonInformation>()
                        .GetListAsync(
                            predicate: x => x.Status.Equals(SalonStatus.Approved)
                            && x.Name.ToLower().Contains(salonName.ToLower())
                            && x.Address.ToLower().Contains(salonAddress.ToLower())
                            && x.ServiceHairs.FirstOrDefault(s => s.ServiceName.Contains(serviceName)) != null,
                            include: query => query.Include(s => s.SalonOwner)
                        );
                // Lọc theo khoảng cách
                var filterSalon = salonQuery
                    .Where(x => DistanceMap.GetDistance((decimal)latitude, (decimal)longtitude, x.Latitude, x.Longitude) <= distance)
                    .ToList(); // Chuyển thành danh sách để tính tổng số mục

                // Tính tổng số mục và số trang
                int total = filterSalon.Count;
                int totalPages = (int)Math.Ceiling((double)total / size);

                // Lấy dữ liệu cho trang hiện tại
                var pagedItems = filterSalon
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToList();

                // Tạo đối tượng phân trang
                salonInformations = new Paginate<SalonInformation>
                {
                    Page = page,
                    Size = size,
                    Total = total,
                    TotalPages = totalPages,
                    Items = pagedItems,
                };
                //salonInformations.Items. = salonInformations.Items.Where(x => DistanceMap.GetDistance((double)latitude!, (double)longtitude!, double.Parse(x.Latitude), double.Parse(x.Longitude)) <= distance);
            }
            else
            {
                salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
                                         .GetPagingListAsync(
                                             predicate: x => x.Status.Equals(SalonStatus.Approved)
                                             && x.Name.ToLower().Contains(salonName.ToLower())
                                             && x.Address.ToLower().Contains(salonAddress.ToLower())
                                             && x.ServiceHairs.FirstOrDefault(s => s.ServiceName.Contains(serviceName)) != null,
                                             include: query => query.Include(s => s.SalonOwner),
                                             page: page,
                                             size: size
                                         );
            }
            var listSalon = _mapper.Map<List<SearchSalonByNameAddressServiceResponse>>(salonInformations.Items);
            //var result = new List<SearchSalonByNameAddressServiceResponse>();

            foreach (var salon in listSalon)
            {
                var services = await _unitOfWork.GetRepository<ServiceHair>().GetListAsync(
                    predicate: x => x.ServiceName.ToLower().Contains(serviceName.ToLower()) && x.SalonInformationId == salon.Id);

                if (services != null && services.Any())
                {
                    // Get Service
                    salon.Services = _mapper.Map<List<SearchSalonServiceResponse>>(services);
                    // Get voucher
                    var vouchers = await _unitOfWork.GetRepository<Voucher>().GetListAsync(predicate: x => x.SalonInformationId == salon.Id);
                    if (vouchers != null)
                    {
                        salon.Vouchers = _mapper.Map<List<SearchSalonVoucherRespnse>>(vouchers);
                    }
                }

                if (longtitude != null && latitude != null && distance != null)
                {
                    salon.Distance = DistanceMap.GetDistance(salon.Latitude, salon.Longitude, (decimal)latitude, (decimal)longtitude);
                }
                else
                {
                    salon.Distance = null;
                }
                TimeOnly timeOnlyNow = TimeOnly.FromDateTime(DateTime.Now);
                string dayOfWeek = DateTime.Now.DayOfWeek.ToString();
                var salonSchedule = await _unitOfWork.GetRepository<Schedule>()
                                           .SingleOrDefaultAsync(
                                                                 predicate: x => x.SalonId == salon.Id
                                                                      && x.DayOfWeek.Equals(dayOfWeek)
                                                                      && x.IsActive);
                if (salonSchedule == null)
                {
                    salon.OperatingStatus = "Không hoạt động vào hôm nay";
                }
                else if (timeOnlyNow >= salonSchedule.StartTime && timeOnlyNow <= salonSchedule.EndTime)
                {
                    salon.OperatingStatus = "Đang hoạt động";
                }
                else
                {
                    salon.OperatingStatus = "Đã qua thời gian làm việc";
                }
            }
            return new Paginate<SearchSalonByNameAddressServiceResponse>()
            {
                Page = salonInformations.Page,
                Size = salonInformations.Size,
                Total = salonInformations.Total,
                TotalPages = salonInformations.TotalPages,
                Items = listSalon,
            };
        }

        public async Task<List<SalonSuggesstionResponse>> GetSalonSuggestion()
        {
            var salons = await _unitOfWork.GetRepository<SalonInformation>()
                .GetListAsync(
                predicate: x => x.Status.Equals(SalonStatus.Approved),
                    orderBy: q => q.OrderByDescending(s => s.Rate)
                                  .ThenByDescending(s => s.TotalReviewer),
                    take: 20
                );

            return _mapper.Map<List<SalonSuggesstionResponse>>(salons);
        }

        public async Task<ReviewRevenueReponse> ReviewRevenue(Guid SalonId, DateTime startDate, DateTime endDate)
        {
            ReviewRevenueReponse result = new ReviewRevenueReponse();
            var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == SalonId);
            if (salon == null)
            {
                throw new NotFoundException($"Không tìm thấy salon/barber shop với id {SalonId}");
            }
            var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(predicate: x => x.SalonInformationId == SalonId);
            foreach (var employee in employees)
            {
                var appointments = await _unitOfWork.GetRepository<Appointment>()
                                                    .GetListAsync(
                                                                    x => x.AppointmentDetails.Any(x => x.SalonEmployeeId == employee.Id)
                                                                    && !x.Status.Equals(AppointmentStatus.Booking)
                                                                    && x.StartDate.Date >= startDate.Date
                                                                    && x.StartDate <= endDate.Date
                                                                 );
                ReviewRevenueEmployee reviewRevenueEmployee = new ReviewRevenueEmployee();
                reviewRevenueEmployee = _mapper.Map<ReviewRevenueEmployee>(employee);

                foreach (var appointment in appointments)
                {
                    switch (appointment.Status)
                    {
                        case AppointmentStatus.Successed:
                            reviewRevenueEmployee.totalSuccessedAppointment++;
                            reviewRevenueEmployee.totalRevuenueEmployee += appointment.TotalPrice;
                            break;
                        case AppointmentStatus.Fail:
                            reviewRevenueEmployee.totalFailedAppointment++;
                            break;
                        case AppointmentStatus.CancelByCustomer:
                            reviewRevenueEmployee.totalCanceledAppointment++;
                            break;
                    }
                }
                result.totalRevenue += reviewRevenueEmployee.totalRevuenueEmployee;
                result.employees.Add(reviewRevenueEmployee);
            }
            return result;
        }

        public async Task<GetSalonInformationResponse>? GetSalonByEmployeeId(Guid id)
        {
            SalonInformation salonInformationResponse = await _unitOfWork.GetRepository<SalonInformation>()
                                                            .SingleOrDefaultAsync(
                                                                predicate: x => x.SalonEmployees.Any(x => x.Id == id),
                                                                include: source => source.Include(s => s.SalonOwner).Include(x => x.Schedules)
                                                            );
            if (salonInformationResponse == null)
                return null;
            return _mapper.Map<GetSalonInformationResponse>(salonInformationResponse);
        }

        public async Task<bool> AddSalonInformationImages(Guid Salonid, AddSalonImagesRequest request)
        {
            var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: p => p.Id == Salonid);
            if (salon == null) throw new Exception("Salon không tồn tại");

            for (int i = 0; i < request.SalonImages.Count; i++)
            {
                var urlImg = await _mediaService.UploadAnImage(request.SalonImages[i], MediaPath.SALONINFORMATION_IMG, salon.Id.ToString() + "/" + i.ToString());
                //var urlVideo = await _mediaservice.UploadAVideo(request.Video, MediaPath.FEEDBACK_VIDEO, newFeedback.Id.ToString());
                StaticFile staticFile = new StaticFile()
                {
                    Id = Guid.NewGuid(),
                    SalonInformationId = salon.Id,
                    Img = urlImg,
                };
                await _unitOfWork.GetRepository<StaticFile>().InsertAsync(staticFile);
            }
            bool isSuccessed = await _unitOfWork.CommitAsync() > 0;
            return isSuccessed;
        }

        public async Task<IPaginate<SalonInformationImagesResponse>> GetSalonInformationImages(Guid salonid, int page, int size)
        {
            var salon = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: p => p.Id == salonid);
            if (salon == null) throw new Exception("Salon không tồn tại");

            var salonImages = await _unitOfWork.GetRepository<StaticFile>().GetPagingListAsync(predicate: p => p.SalonInformationId == salonid, page: page, size: size);

            if (salonImages == null) throw new Exception("Salon không có hình");


            var salonInformationResponses = new Paginate<SalonInformationImagesResponse>()
            {
                Page = salonImages.Page,
                Size = salonImages.Size,
                Total = salonImages.Total,
                TotalPages = salonImages.TotalPages,
                Items = _mapper.Map<IList<SalonInformationImagesResponse>>(salonImages.Items),
            };
            return salonInformationResponses;
        }

        public async Task<bool> DeleteSalonInformationImages(DeleteImagesRequest request)
        {
            bool isDeleted = false;
            if (request != null)
            {
                foreach (var imageid in request.ImagesId)
                {
                    var image = await _unitOfWork.GetRepository<StaticFile>().SingleOrDefaultAsync(predicate: p => p.Id == imageid);
                    _unitOfWork.GetRepository<StaticFile>().DeleteAsync(image);
                }
                isDeleted = await _unitOfWork.CommitAsync() > 0;

            }
            return isDeleted;
        }

        private async Task<decimal> CaculateRevenue(int endDay, int month, int year, Guid SalonId, string statusAppointment)
        {
            DateTime StartTime = new DateTime(year, month, 1);
            DateTime EndTime = new DateTime(year, month, endDay);
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                                .GetListAsync(
                                                                predicate: x => x.AppointmentDetails.Any(s => s.SalonEmployee.SalonInformationId == SalonId)
                                                                            && x.StartDate.Date >= StartTime.Date && x.StartDate <= EndTime.Date && x.Status.Equals(statusAppointment)
                                                             );
            decimal result = 0;
            foreach (var appointment in appointments)
            {
                result += appointment.TotalPrice;
            }
            return result;
        }

        public async Task<CompileRevenueSalonByYearResponse> CompileRevenueSalonByYear(Guid SalonId, int Year)
        {
            CompileRevenueSalonByYearResponse result = new CompileRevenueSalonByYearResponse();
            decimal totalInSideRevenue = 0;
            decimal totalOutSideRevenue = 0;
            decimal totalRevenue = 0;
            for (int i = 1; i <= 12; i++)
            {
                decimal tempInsideRevenue = 0, tempOutSideRevenue = 0;
                switch (i)
                {
                    case 1:
                        tempInsideRevenue = await CaculateRevenue(31, 1, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(31, 1, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 2:
                        if((Year % 4 == 0 && Year % 100 != 0) || (Year % 400 == 0)){
                            tempInsideRevenue = await CaculateRevenue(29, 2, Year, SalonId, AppointmentStatus.Successed);
                            tempOutSideRevenue = await CaculateRevenue(29, 2, Year, SalonId, AppointmentStatus.OutSide);
                        }
                        else
                        {
                            tempInsideRevenue = await CaculateRevenue(28, 2, Year, SalonId, AppointmentStatus.Successed);
                            tempOutSideRevenue = await CaculateRevenue(28, 2, Year, SalonId, AppointmentStatus.OutSide);
                        }
                        break;
                    case 3:
                        tempInsideRevenue = await CaculateRevenue(31, 3, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(31, 3, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 4:
                        tempInsideRevenue = await CaculateRevenue(30, 4, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(30, 4, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 5:
                        tempInsideRevenue = await CaculateRevenue(31, 5, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(31, 5, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 6:
                        tempInsideRevenue = await CaculateRevenue(30, 6, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(30, 6, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 7:
                        tempInsideRevenue = await CaculateRevenue(31, 7, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(31, 7, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 8:
                        tempInsideRevenue = await CaculateRevenue(31, 8, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(31, 8, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 9:
                        tempInsideRevenue = await CaculateRevenue(30, 9, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(30, 9, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 10:
                        tempInsideRevenue = await CaculateRevenue(31, 10, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(31, 10, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 11:
                        tempInsideRevenue = await CaculateRevenue(30, 11, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(30, 11, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                    case 12:
                        tempInsideRevenue = await CaculateRevenue(31, 12, Year, SalonId, AppointmentStatus.Successed);
                        tempOutSideRevenue = await CaculateRevenue(31, 12, Year, SalonId, AppointmentStatus.OutSide);
                        break;
                }

                totalInSideRevenue += tempInsideRevenue;
                totalOutSideRevenue += tempOutSideRevenue;
                result.revenuewStatistics.Add(new RevenueStatistic()
                {
                    InSideRevenue = tempInsideRevenue,
                    OutSideRevenue = tempOutSideRevenue,
                    TotalRevenue = tempInsideRevenue + tempOutSideRevenue,
                    Month = $"Tháng {i}"
                });
            }

            totalRevenue = totalInSideRevenue + totalOutSideRevenue;
            if (totalRevenue == 0)
            {
                result.OutSideRevenuePercent = 0;
                result.InSideRevenuePercent = 0;
            }
            else
            {
                result.OutSideRevenuePercent = (totalOutSideRevenue / totalRevenue) * 100;
                result.InSideRevenuePercent = (totalInSideRevenue/ totalRevenue) * 100;
            }
            return result;
        }

        private async Task<long> CaculateNumberOfAppointment(int endDay, int month, int year, Guid SalonId, string statusAppointment)
        {
            DateTime StartTime = new DateTime(year, month, 1);
            DateTime EndTime = new DateTime(year, month, endDay);
            var appointments = await _unitOfWork.GetRepository<Appointment>()
                                                .GetListAsync(
                                                                predicate: x => x.AppointmentDetails.Any(s => s.SalonEmployee.SalonInformationId == SalonId)
                                                                            && x.StartDate.Date >= StartTime.Date && x.StartDate <= EndTime.Date && x.Status.Equals(statusAppointment)
                                                             );
            return appointments.Count();
        }

        public async Task<CompileAppointmentSalonByYearResponse> CompileAppointmentSalonByYear(Guid SalonId, int Year)
        {
            CompileAppointmentSalonByYearResponse result = new CompileAppointmentSalonByYearResponse();
            long totalInSideAppointment = 0, totalOutSideAppointment = 0, totalCancelAppointment = 0, totalFailedAppointment = 0;
            for (int i = 1; i <= 12; i++)
            {
                int endDay = 0;
                switch (i)
                {
                    case 1:
                        endDay = 31;
                        break;
                    case 2:
                        if ((Year % 4 == 0 && Year % 100 != 0) || (Year % 400 == 0))
                        {
                            endDay = 29;
                        }
                        else
                        {
                            endDay = 28;
                        }
                        break;
                    case 3:
                        endDay = 31;
                        break;
                    case 4:
                        endDay = 30;
                        break;
                    case 5:
                        endDay = 31;
                        break;
                    case 6:
                        endDay = 30;
                        break;
                    case 7:
                        endDay = 31;
                        break;
                    case 8:
                        endDay = 31;
                        break;
                    case 9:
                        endDay = 30;
                        break;
                    case 10:
                        endDay = 31;
                        break;
                    case 11:
                        endDay = 30;
                        break;
                    case 12:
                        endDay = 31;
                        break;
                }
                long tempInSideAppointment = await CaculateNumberOfAppointment(endDay, i, Year, SalonId, AppointmentStatus.Successed);
                long tempOutSideAppointment = await CaculateNumberOfAppointment(endDay, i, Year, SalonId, AppointmentStatus.OutSide);
                long tempCancelAppointment = await CaculateNumberOfAppointment(endDay, i, Year, SalonId, AppointmentStatus.CancelByCustomer);
                long tempFailedAppointment = await CaculateNumberOfAppointment(endDay, i, Year, SalonId, AppointmentStatus.Fail);

                totalInSideAppointment += tempInSideAppointment;
                totalOutSideAppointment += tempOutSideAppointment;
                totalCancelAppointment += tempCancelAppointment;
                totalFailedAppointment += tempFailedAppointment;
                result.revenuewStatistics.Add(new AppointmentStatistic()
                {
                    CancelAppointment = tempCancelAppointment,
                    FailedAppointment = tempFailedAppointment,
                    InSideAppointment = tempInSideAppointment,
                    OutSideAppointment = tempOutSideAppointment,
                    TotalAppointment = tempInSideAppointment + tempOutSideAppointment + tempCancelAppointment + tempFailedAppointment,
                    Month = $"Tháng {i}"
                });
            }

            long totalAppointment = totalInSideAppointment + totalOutSideAppointment + totalCancelAppointment + totalFailedAppointment;
            if (totalAppointment == 0)
            {
                result.OutSideAppointmentPercent = 0;
                result.InSideAppointmentPercent = 0;
                result.FailedAppointmentPercent = 0;
                result.CancelAppointmentPercent = 0;
            }
            else
            {
                result.OutSideAppointmentPercent = ((decimal)totalOutSideAppointment /totalAppointment)*100;
                result.InSideAppointmentPercent = ((decimal)totalInSideAppointment /totalAppointment)*100;
                result.FailedAppointmentPercent = ((decimal)totalFailedAppointment / totalAppointment)*100;
                result.CancelAppointmentPercent = ((decimal)totalCancelAppointment /totalAppointment)*100;
            }
            return result;
        }

<<<<<<< HEAD
        public async Task<RevenueStatistics> RevenueStatistics(Guid salonId, DateTime? startDate, DateTime? endDate)
        {
            var predicate = PredicateBuilder.New<Appointment>(true);


            predicate = predicate.And(x =>
                x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId));
            if (startDate.HasValue && endDate.HasValue)
            {
                predicate = predicate.And(x => x.StartDate >= startDate && x.StartDate <= endDate);
            } else
            {
                predicate = predicate.And(x =>  x.StartDate <= DateTime.Now);
            }
            IEnumerable<Appointment> appointments;

            appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetListAsync(
                    predicate: predicate,
                    include: query => query.Include(a => a.Customer)
                                           .Include(a => a.AppointmentDetails)
                                               .ThenInclude(ad => ad.SalonEmployee)
                                                   .ThenInclude(se => se.SalonInformation),
                    orderBy: query => query.OrderBy(a => a.AppointmentDetails!
                        .OrderByDescending(ad => ad.StartTime)!
                        .FirstOrDefault()!.StartTime)
                );

            var revenueStatistics = new RevenueStatistics
            {
                TotalRevenue = appointments.Where(a => a.Status == AppointmentStatus.Successed || a.Status == AppointmentStatus.OutSide).Sum(a => a.TotalPrice),
                OutsideRevenue = appointments.Where(a => a.Status == AppointmentStatus.OutSide).Sum(a => a.TotalPrice),
                PlatformRevenue = appointments.Where(a => a.Status == AppointmentStatus.Successed).Sum(a => a.TotalPrice),
                NumberOfOutsideAppointment = appointments.Count(a => a.Status == AppointmentStatus.OutSide),
                NumberOfPlatformAppointment = appointments.Count(a => a.Status == AppointmentStatus.Successed),
                NumberOfCancelAppointment = appointments.Count(a => a.Status == AppointmentStatus.CancelByCustomer),
                NumberOfFailedAppointment = appointments.Count(a => a.Status == AppointmentStatus.Fail)
            };

            return revenueStatistics;
        }

        public async Task<List<ServiceStatistics>> ServiceStatistics(Guid salonId, DateTime? startDate, DateTime? endDate)
        {
            var predicate = PredicateBuilder.New<Appointment>(true);


            predicate = predicate.And(x =>
                x.AppointmentDetails.Any(ad => ad.SalonEmployee.SalonInformationId == salonId));
            if (startDate.HasValue && endDate.HasValue)
            {
                predicate = predicate.And(x => x.StartDate >= startDate && x.StartDate <= endDate);
            }
            else
            {
                predicate = predicate.And(x => x.StartDate <= DateTime.Now);
            }
            IEnumerable<Appointment> appointments;

            appointments = await _unitOfWork.GetRepository<Appointment>()
                .GetListAsync(
                    predicate: predicate.And(x => x.Status == AppointmentStatus.OutSide || x.Status == AppointmentStatus.Successed),
                    include: query => query.Include(a => a.Customer)
                                           .Include(a => a.AppointmentDetails)
                                               .ThenInclude(ad => ad.SalonEmployee)
                                                   .ThenInclude(se => se.SalonInformation),
                    orderBy: query => query.OrderBy(a => a.AppointmentDetails!
                        .OrderByDescending(ad => ad.StartTime)!
                        .FirstOrDefault()!.StartTime)
                );
            var services = await _unitOfWork.GetRepository<ServiceHair>()
                .GetListAsync(predicate: p => p.SalonInformationId == salonId);

            List<ServiceStatistics> list = new List<ServiceStatistics>();
            foreach (var service in services)
            {
                var appointmentDetailsWithService = appointments.SelectMany(a => a.AppointmentDetails).Where(ad => ad.ServiceName == service.ServiceName);

                var numberOfUses = appointmentDetailsWithService.Count();

                var revenueFromService = appointmentDetailsWithService.Sum(ad => ad.PriceServiceHair);

                var numberOfCustomers = appointments
                    .Where(a => a.AppointmentDetails.Any(ad => ad.ServiceName == service.ServiceName))
                    .Select(a => a.Customer.Id)
                    .Distinct()
                    .Count();

                list.Add(new ServiceStatistics
                {
                    ServiceName = service.ServiceName,
                    NumberOfUses = numberOfUses,
                    RevenueFromService = revenueFromService,
                    NumberOfCustomers = numberOfCustomers
                });
            }
            return list;
=======
        public async Task<EmployeeStatictisResponse> CompileEmployeeRevenue(Guid salonId, DateTime startDate, DateTime endDate, string? filter)
        {
            EmployeeStatictisResponse result = new EmployeeStatictisResponse();
            var employees = await _unitOfWork.GetRepository<SalonEmployee>().GetListAsync(predicate: x=>x.SalonInformationId == salonId && x.IsActive);
            foreach (var employee in employees)
            {
                ICollection<AppointmentDetail> appointmentDetails = await _unitOfWork.GetRepository<AppointmentDetail>()
                                                          .GetListAsync(
                                                                        predicate: x=>x.SalonEmployeeId == employee.Id 
                                                                                    && (x.Appointment.Status.Equals(AppointmentStatus.OutSide) || x.Appointment.Status.Equals(AppointmentStatus.Successed)),
                                                                        include: x=>x.Include(s=>s.Appointment)
                                                                       );
                var uniqueCustomerCount = appointmentDetails
                                            .Select(x => x.Appointment.CustomerId) 
                                            .Distinct()                            
                                            .Count();
                decimal revenue = appointmentDetails
                                                .Where(ad => ad.PriceServiceHair.HasValue) 
                                                .Sum(ad => ad.PriceServiceHair.Value);
                result.EmployeeStatictis.Add(new EmployeeStatictis()
                {
                    FullName = employee.FullName,
                    Id = employee.Id,
                    NumberOfService = appointmentDetails.Count(),
                    NumberOfUsers = uniqueCustomerCount,
                    Revenue = revenue
                });
            }

            if (filter.IsNullOrEmpty())
            {
                filter = "";
            }
            switch (filter)
            {
                case "Số lượng dịch vụ tăng dần":
                    result.EmployeeStatictis.OrderBy(x => x.NumberOfService);
                    break;
                case "Số lượng dịch vụ giảm dần":
                    result.EmployeeStatictis.OrderByDescending(x => x.NumberOfService);
                    break;
                case "Số lượng khách tăng dần":
                    result.EmployeeStatictis.OrderBy(x => x.NumberOfUsers);
                    break;
                case "Số lượng khách giảm dần":
                    result.EmployeeStatictis.OrderByDescending(x => x.NumberOfUsers);
                    break;
                case "Số doanh thu tăng dần":
                    result.EmployeeStatictis.OrderBy(x => x.Revenue);
                    break;
                case "Số doanh thu giảm dần":
                    result.EmployeeStatictis.OrderByDescending(x => x.NumberOfUsers);
                    break;
                default:
                    result.EmployeeStatictis.OrderBy(x => x.NumberOfService);
                    break;
            }
            return result;
>>>>>>> 1a9f71d06e2c8aae4975f2233820a3f0201e2961
        }
    }
}