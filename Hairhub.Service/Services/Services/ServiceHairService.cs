using AutoMapper;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Dtos.Requests.ServiceHairs;
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
            var existSalon = _unitOfWork.GetRepository<SalonInformation>().SingleOrDefaultAsync(predicate: x => x.Id == createServiceHairRequest.SalonInformationId);
            if (existSalon == null)
            {
                throw new NotFoundException($"Not found salon with id {createServiceHairRequest.SalonInformationId}");
            }
            var serviceHair = _mapper.Map<ServiceHair>(createServiceHairRequest);
            serviceHair.Id = Guid.NewGuid();
            var url = await _mediaService.UploadAnImage(createServiceHairRequest.Img, MediaPath.SERVICE_HAIR, serviceHair.Id.ToString());
            serviceHair.Img = url;
            await _unitOfWork.GetRepository<ServiceHair>().InsertAsync(serviceHair);
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            return isInsert;
        }

        public async Task<bool> DeleteServiceHairById(Guid id)
        {
            var serviceHair = await _unitOfWork.GetRepository<ServiceHair>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            var appointment = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(predicate: p => p.ServiceHairId == serviceHair.Id);

            if (serviceHair == null)
            {
                throw new NotFoundException("Dịch vụ này không có");
            }
            if (appointment != null)
            {
                throw new NotFoundException("Dịch vụ này không thể xóa vì đã có lịch hẹn đặt");
            }
            serviceHair.IsActive = false;
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

        public async Task<IPaginate<GetServiceHairResponse>> GetServiceHairBySalonIdPaging(Guid id, int page, int size, string? orderby, bool? filter, string? search)
        {
            
            var predicate = PredicateBuilder.New<ServiceHair>(s => s.SalonInformationId == id);

            if (!string.IsNullOrWhiteSpace(search))
            {
                predicate = predicate.And(s => s.ServiceName.Contains(search));
            }

            if (filter.HasValue)
            {
                predicate = predicate.And(x => x.IsActive == filter.Value);
            }


            Func<IQueryable<ServiceHair>, IOrderedQueryable<ServiceHair>> orderBy = null;

            if (!string.IsNullOrWhiteSpace(orderby))
            {
                if (orderby.Equals("giá tăng dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderBy(s => s.Price);
                }
                else if (orderby.Equals("giá giảm dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderByDescending(s => s.Price);
                }
                else if (orderby.Equals("thời gian tăng dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderBy(s => s.Time);
                }
                else if (orderby.Equals("thời gian giảm dần", StringComparison.OrdinalIgnoreCase))
                {
                    orderBy = q => q.OrderByDescending(s => s.Time);
                }
            }

            
            var serviceHairs = await _unitOfWork.GetRepository<ServiceHair>()
                                                .GetPagingListAsync(
                                                    predicate: predicate,
                                                    orderBy: orderBy,
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

            if (!string.IsNullOrEmpty(updateServiceHairRequest.ServiceName))
            {
                serviceHair.ServiceName = updateServiceHairRequest.ServiceName;
            }

            if (!string.IsNullOrEmpty(updateServiceHairRequest.Description))
            {
                serviceHair.Description = updateServiceHairRequest.Description;
            }

            if (updateServiceHairRequest.Price.HasValue)
            {
                serviceHair.Price = updateServiceHairRequest.Price.Value;
            }

            if (updateServiceHairRequest.Img != null)
            {
                serviceHair.Img = await _mediaService.UploadAnImage(updateServiceHairRequest.Img, MediaPath.SERVICE_HAIR, serviceHair.Id.ToString());
            }

            if (updateServiceHairRequest.Time.HasValue)
            {
                serviceHair.Time = updateServiceHairRequest.Time.Value;
            }

            if (updateServiceHairRequest.IsActive.HasValue)
            {

                serviceHair.IsActive = updateServiceHairRequest.IsActive.Value;
            }

            _unitOfWork.GetRepository<ServiceHair>().UpdateAsync(serviceHair);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<bool> UpdateServiceHairofEmployee(Guid employeeId, List<Guid> addnewserviceid, List<Guid> serviceidRemove)
        {
            var employee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: p => p.Id == employeeId);
            if (employee == null) {

                throw new Exception("Không tồn tại nhân viên này");
            }
            if (serviceidRemove != null)
            {
                
                foreach (var serviceid in serviceidRemove) {
                    
                    var servicehair = await _unitOfWork.GetRepository<ServiceEmployee>().SingleOrDefaultAsync(predicate: p => p.ServiceHairId == serviceid);

                    var appointment = await _unitOfWork.GetRepository<AppointmentDetail>().GetListAsync(
                                    predicate: p => p.SalonEmployeeId == employeeId 
                                                    && p.ServiceHairId == serviceid 
                                                    && p.Status == AppointmentStatus.Booking
                                                    && p.StartTime > DateTime.UtcNow);
                    if (appointment.Count > 0) {
                        throw new Exception("Không thể thay đổi dịch vụ vì còn lịch hẹn khác");
                    }

                    _unitOfWork.GetRepository<ServiceEmployee>().DeleteAsync(servicehair);
                }
                                
            }
            if(addnewserviceid != null)
            {
                List<ServiceEmployee> newservicehairs = new List<ServiceEmployee>();
                foreach (var newserviceid  in addnewserviceid)
                {

                    var servicehairinSalon = await _unitOfWork.GetRepository<ServiceHair>().SingleOrDefaultAsync(predicate: p => p.Id == newserviceid && p.SalonInformationId == employee.SalonInformationId);
                    if(servicehairinSalon == null)
                    {
                        throw new Exception("không có dịch vụ này trong salon");
                    }
                    ServiceEmployee newservicehair = new ServiceEmployee
                    {
                        Id = Guid.NewGuid(),
                        ServiceHairId = servicehairinSalon.Id,
                        SalonEmployeeId = employeeId,
                    };
                    newservicehairs.Add(newservicehair);
                }
                await _unitOfWork.GetRepository<ServiceEmployee>().InsertRangeAsync(newservicehairs);
                
            }
            bool isUpdated = _unitOfWork.Commit() > 0;
            return isUpdated;
        }
    }
}
