using AutoMapper;
using Hairhub.Common.ThirdParties.Contract;
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
using Org.BouncyCastle.Asn1.Ocsp;
using System.Linq;

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

        public async Task<IPaginate<GetSalonInformationResponse>> GetSalonByStatus(string? status, int page, int size)
        {
            IPaginate<SalonInformation> salonInformations;
            if (status == null || String.IsNullOrWhiteSpace(status))
            {
                salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
                                                        .GetPagingListAsync(
                                                           include: query => query.Include(s => s.SalonOwner),
                                                           page: page,
                                                           size: size
                                                        );
            }
            else
            {
                salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
                                        .GetPagingListAsync(
                                            predicate: x => x.Status.Equals(status),
                                           include: query => query.Include(s => s.SalonOwner),
                                           page: page,
                                           size: size
                                        );
            }
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

            if (!string.IsNullOrEmpty(updateSalonInformationRequest.Longitude))
            {
                salonInformation.Longitude = updateSalonInformationRequest.Longitude;
            }

            if (!string.IsNullOrEmpty(updateSalonInformationRequest.Latitude))
            {
                salonInformation.Latitude = updateSalonInformationRequest.Latitude;
            }

            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;


           
        }

        public async Task<IPaginate<SearchSalonByNameAddressServiceResponse>> SearchSalonByNameAddressService(int page, int size, string? serviceName = "", 
                                        string? salonAddress = "", string? salonName = "", double? latitude = 0, double? longtitude = 0, double? distance = 0)
        {
            //if (serviceName == null && salonAddress == null && salonName == null)
            //{
            //    return new Paginate<SearchSalonByNameAddressServiceResponse>()
            //    {
            //        Page = page,
            //        Size = size,
            //        Total = 0,
            //        TotalPages = 0,
            //        Items = new List<SearchSalonByNameAddressServiceResponse>(),
            //    };
            //}
            serviceName = serviceName?.Trim() ?? string.Empty;
            salonAddress = salonAddress?.Trim() ?? string.Empty;
            salonName = salonName?.Trim() ?? string.Empty;

            var salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
                                                     .GetListAsync(
                                                         predicate: x => x.Status.Equals(SalonStatus.Approved) 
                                                         && x.Name.ToLower().Contains(salonName.ToLower()) 
                                                         && x.Address.ToLower().Contains(salonAddress.ToLower()),
                                                         include: query => query.Include(s => s.SalonOwner)
                                                     );
            if (latitude!=null && longtitude!=null && distance!=null)
            {
                salonInformations.Where(x => DistanceMap.GetDistance((double)latitude!, (double)longtitude!, double.Parse(x.Latitude), double.Parse(x.Longitude)) <= distance);
            }
            var listSalon = _mapper.Map<List<SearchSalonByNameAddressServiceResponse>>(salonInformations);
            var result = new List<SearchSalonByNameAddressServiceResponse>();

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
                    result.Add(salon);
                }
            }
            return new Paginate<SearchSalonByNameAddressServiceResponse>()
            {
                Page = page,
                Size = size,
                Total = result.Count,
                TotalPages = (int)Math.Ceiling((double)result.Count / size),
                Items = result,
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
    }
}
