using AutoMapper;
using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Responses.SalonInformations;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;

namespace Hairhub.Service.Services.Services
{
    public class SalonInformationService : ISalonInformationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;

        public SalonInformationService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
        }

        public async Task<bool> ActiveSalonInformation(Guid id)
        {
            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonInformation == null)
            {
                throw new NotFoundException("SalonInformation not found!");
            }
            salonInformation.IsActive = true;
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
            await _unitOfWork.GetRepository<SalonInformation>().InsertAsync(salonInformation);
            foreach (var scheduleRequest in createSalonInformationRequest.SalonInformationSchedules)
            {
                var newSchedule = new Schedule
                {
                    Id = Guid.NewGuid(),
                    Date = scheduleRequest.Date,
                    SalonId = salonInformation.Id,
                    StartTime = TimeOnly.Parse(scheduleRequest.StartTime),
                    EndTime = TimeOnly.Parse(scheduleRequest.EndTime),
                    IsActive = true
                };
                await _unitOfWork.GetRepository<Schedule>().InsertAsync(newSchedule);
            }

            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateSalonInformationResponse>(salonInformation);
        }

        public async Task<bool> DeleteSalonInformationById(Guid id)
        {
            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonInformation == null)
            {
                throw new NotFoundException("SalonInformation not found!");
            }
            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<IPaginate<GetSalonInformationResponse>> GetAllSalonInformation(int page, int size)
        {
            var salonInformations = await _unitOfWork.GetRepository<SalonInformation>()
           .GetPagingListAsync(
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
                    include: source => source.Include(s => s.SalonOwner)
                 );
            if (salonInformationResponse == null)
                return null;
            return _mapper.Map<GetSalonInformationResponse>(salonInformationResponse);
        }

        public async Task<bool> UpdateSalonInformationById(Guid id, UpdateSalonInformationRequest updateSalonInformationRequest)
        {
            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonInformation == null)
            {
                throw new NotFoundException("SalonInformation not found!");
            }
            salonInformation = _mapper.Map<SalonInformation>(updateSalonInformationRequest);
            _unitOfWork.GetRepository<SalonInformation>().UpdateAsync(salonInformation);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
    }
}
