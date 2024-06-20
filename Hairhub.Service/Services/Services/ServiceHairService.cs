using AutoMapper;
using Hairhub.Domain.Dtos.Requests.ServiceHairs;
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
    public class ServiceHairService : IServiceHairService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediaService _mediaService;

        public ServiceHairService(IUnitOfWork unitOfWork, IMapper mapper, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediaService = mediaService;
        }

        public async Task<bool> ActiveServiceHair(Guid id)
        {
            var serviceHair = await _unitOfWork.GetRepository<ServiceHair>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (serviceHair == null)
            {
                throw new NotFoundException("ServiceHair not found!");
            }
            serviceHair.IsActive = true;
            _unitOfWork.GetRepository<ServiceHair>().UpdateAsync(serviceHair);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<bool> CreateServiceHair(CreateServiceHairRequest createServiceHairRequest)
        {
            var existSalon = _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x=>x.Id == createServiceHairRequest.SalonInformationId);
            if (existSalon == null)
            {
                throw new NotFoundException($"Not found salon with id {createServiceHairRequest.SalonInformationId}");
            }
            var serviceHair = _mapper.Map<ServiceHair>(createServiceHairRequest);
            serviceHair.Id = Guid.NewGuid();
            var url = await _mediaService.UploadAnImage(createServiceHairRequest.Img, MediaPath.SERVICE_HAIR, serviceHair.Id.ToString());
            serviceHair.Img = url;
            await _unitOfWork.GetRepository<ServiceHair>().InsertAsync(serviceHair);
            bool isInsert = await _unitOfWork.CommitAsync()>0;
            return isInsert;
        }

        public async Task<bool> DeleteServiceHairById(Guid id)
        {
            var serviceHair = await _unitOfWork.GetRepository<ServiceHair>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (serviceHair == null)
            {
                throw new NotFoundException("ServiceHair not found!");
            }
            _unitOfWork.GetRepository<ServiceHair>().UpdateAsync(serviceHair);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<IPaginate<GetServiceHairResponse>> GetAllServiceHair(int page, int size)
        {
            var serviceHairs = await _unitOfWork.GetRepository<ServiceHair>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.ServiceEmployees),
               page: page,
               size: size
           );

            var serviceHairResponses = new Paginate<GetServiceHairResponse>()
            {
                Page = serviceHairs.Page,
                Size = serviceHairs.Size,
                Total = serviceHairs.Total,
                TotalPages = serviceHairs.TotalPages,
                Items = _mapper.Map<IList<GetServiceHairResponse>>(serviceHairs.Items),
            };
            return serviceHairResponses;
        }

        public async Task<GetServiceHairResponse>? GetServiceHairById(Guid id)
        {
            ServiceHair serviceHairResponse = await _unitOfWork
                .GetRepository<ServiceHair>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: source => source.Include(s => s.ServiceEmployees)
                 );
            if (serviceHairResponse == null)
                return null;
            return _mapper.Map<GetServiceHairResponse>(serviceHairResponse);
        }

        public async Task<IEnumerable<GetServiceHairResponse>> GetServiceHairBySalonInformationId(Guid salonInformationId)
        {
            var services = await _unitOfWork.GetRepository<ServiceHair>()
                .GetListAsync(predicate: s => s.SalonInformationId == salonInformationId);

            return _mapper.Map<IEnumerable<GetServiceHairResponse>>(services);
        }

        public async Task<bool> UpdateServiceHairById(Guid id, UpdateServiceHairRequest updateServiceHairRequest)
        {
            var serviceHair = await _unitOfWork.GetRepository<ServiceHair>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (serviceHair == null)
            {
                throw new NotFoundException("ServiceHair not found!");
            }
            serviceHair = _mapper.Map<ServiceHair>(updateServiceHairRequest);
            _unitOfWork.GetRepository<ServiceHair>().UpdateAsync(serviceHair);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
    }
}
