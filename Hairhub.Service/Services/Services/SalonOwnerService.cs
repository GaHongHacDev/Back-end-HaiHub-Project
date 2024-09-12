using AutoMapper;
using Hairhub.Domain.Dtos.Requests.SalonOwners;
using Hairhub.Domain.Dtos.Responses.SalonOwners;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Hairhub.Service.Services.Services
{
    public class SalonOwnerService : ISalonOwnerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalonOwnerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CreateSalonOwnerResponse> CreateSalonOwner(CreateSalonOwnerRequest createSalonOwnerRequest)
        {
            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createSalonOwnerRequest.AccountId));
            if (account == null)
            {
                throw new Exception("AccountId not found");
            }
            var salonOwner = _mapper.Map<SalonOwner>(createSalonOwnerRequest);
            await _unitOfWork.GetRepository<SalonOwner>().InsertAsync(salonOwner);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateSalonOwnerResponse>(salonOwner);
        }

        public async Task<bool> DeleteSalonOwnerById(Guid id)
        {
            var salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonOwner == null)
            {
                throw new NotFoundException("SalonOwner not found!");
            }
            _unitOfWork.GetRepository<SalonOwner>().DeleteAsync(salonOwner);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<IPaginate<GetSalonOwnerResponse>> GetAllSalonOwner(string? email, bool? status, int page, int size)
        {
            var salonowners = await _unitOfWork.GetRepository<SalonOwner>()
        .GetPagingListAsync(
            predicate: c =>
                (string.IsNullOrEmpty(email) || c.Email.Contains(email)) &&
                (!status.HasValue || c.Account.IsActive == status.Value),  // Nullable bool handling
            include: query => query.Include(s => s.Account),
            page: page,
            size: size
        );

            var salonownerResponses = new Paginate<GetSalonOwnerResponse>()
            {
                Page = salonowners.Page,
                Size = salonowners.Size,
                Total = salonowners.Total,
                TotalPages = salonowners.TotalPages,
                Items = _mapper.Map<IList<GetSalonOwnerResponse>>(salonowners.Items),
            };
            return salonownerResponses;
        }

        public async Task<GetSalonOwnerResponse>? GetSalonOwnerById(Guid id)
        {
            SalonOwner salonownerResponse = await _unitOfWork
                .GetRepository<SalonOwner>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: source => source.Include(a => a.Account)
                 );
            if (salonownerResponse == null)
                return null;
            return _mapper.Map<GetSalonOwnerResponse>(salonownerResponse);
        }

        public async Task<bool> UpdateSalonOwnerById(Guid id, UpdateSalonOwnerRequest updateSalonOwnerRequest)
        {
            var salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonOwner == null)
            {
                throw new NotFoundException("SalonOwner not found!");
            }
            salonOwner = _mapper.Map<SalonOwner>(updateSalonOwnerRequest);
            _unitOfWork.GetRepository<SalonOwner>().UpdateAsync(salonOwner);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
    }
}
