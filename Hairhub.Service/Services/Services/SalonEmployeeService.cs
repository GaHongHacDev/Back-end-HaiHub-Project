using AutoMapper;
using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Responses.SalonEmployees;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace Hairhub.Service.Services.Services
{
    public class SalonEmployeeService : ISalonEmployeeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SalonEmployeeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<CreateSalonEmployeeResponse> CreateSalonEmployee(CreateSalonEmployeeRequest createSalonEmployeeRequest)
        {
            var salonInformation = await _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id.Equals(createSalonEmployeeRequest.SalonInformationId));
            if (salonInformation == null)
            {
                throw new Exception($"salonInformation not found with id: {createSalonEmployeeRequest.SalonInformationId}");
            }
            var salonEmployee = _mapper.Map<SalonEmployee>(createSalonEmployeeRequest);
            await _unitOfWork.GetRepository<SalonEmployee>().InsertAsync(salonEmployee);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CreateSalonEmployeeResponse>(salonEmployee);
        }

        public async Task<bool> DeleteSalonEmployeeById(Guid id)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonEmployee == null)
            {
                throw new NotFoundException("SalonEmployee not found!");
            }
            _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(salonEmployee);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
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

        public async Task<GetSalonEmployeeResponse>? GetSalonEmployeeById(Guid id)
        {
            SalonEmployee salonEmployeeResponse = await _unitOfWork
                .GetRepository<SalonEmployee>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id),
                    include: source => source.Include(s => s.SalonInformation)
                 );
            if (salonEmployeeResponse == null)
                return null;
            return _mapper.Map<GetSalonEmployeeResponse>(salonEmployeeResponse);
        }

        public async Task<GetSalonEmployeeResponse>? GetSalonEmployeeBySalonInformationId(Guid? SalonInformationId)
        {
            SalonEmployee salonEmployeeResponse = await _unitOfWork
                .GetRepository<SalonEmployee>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(SalonInformationId),
                    include: source => source.Include(s => s.SalonInformation)
                 );
            if (salonEmployeeResponse == null)
                return null;
            return _mapper.Map<GetSalonEmployeeResponse>(salonEmployeeResponse);
        }

        public async Task<bool> UpdateSalonEmployeeById(Guid id, UpdateSalonEmployeeRequest updateSalonEmployeeRequest)
        {
            var salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            if (salonEmployee == null)
            {
                throw new NotFoundException("SalonEmployee not found!");
            }
            salonEmployee = _mapper.Map<SalonEmployee>(updateSalonEmployeeRequest);
            _unitOfWork.GetRepository<SalonEmployee>().UpdateAsync(salonEmployee);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }
    }
}
