using AutoMapper;
using Hairhub.Domain.Dtos.Requests.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Entitities;
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

        public ServiceHairService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<CreateServiceHairResponse> CreateServiceHair(CreateServiceHairRequest createServiceHairRequest)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createServiceHairRequest.SalonInformationId));
            if (account == null)
            {
                throw new Exception("AccountId not found");
            }
            var serviceHair = _mapper.Map<ServiceHair>(createServiceHairRequest);
            await _unitOfWork.GetRepository<ServiceHair>().InsertAsync(serviceHair);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateServiceHairResponse>(serviceHair);
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
               include: query => query.Include(s => s.SalonInformation),
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
                    include: source => source.Include(s => s.SalonInformation)
                 );
            if (serviceHairResponse == null)
                return null;
            return _mapper.Map<GetServiceHairResponse>(serviceHairResponse);
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
