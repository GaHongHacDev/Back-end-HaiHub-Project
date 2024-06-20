using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Responses.Config;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class ConfigService : IConfigService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly IMapper _mapper;
        public ConfigService(IUnitOfWork unitofwork, IMapper mapper)
        {
            _unitofwork = unitofwork;
            _mapper = mapper;
        }

        public async Task<CreateConfigResponse> CreateConfigAsync(CreateConfigRequest request)
        {
            var config = new Config()
            {
                PakageName = request.PakageName,
                PakageFee = request.PakageFee,
                Description = request.Description,
                DateCreate = DateTime.Now,
                IsActive = true,
            };

            await _unitofwork.GetRepository<Config>().InsertAsync(config);
            await _unitofwork.CommitAsync();
            return _mapper.Map<CreateConfigResponse>(config);
        }

        

        public async Task<bool> DeleteConfigAsync(Guid id)
        {
            var existconfig = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(predicate: e => e.Id.Equals(id));
            if (existconfig == null)
            {
                throw new NotFoundException("ServiceHair not found!");
            }
            _unitofwork.GetRepository<Config>().DeleteAsync(existconfig);
            bool isUpdate = await _unitofwork.CommitAsync() > 0;
            return isUpdate;

        }

        public async Task<IPaginate<GetConfigResponse>> GetConfigAsync(int page, int size)
        {
            IPaginate<GetConfigResponse> config = await _unitofwork.GetRepository<Config>()
                 .GetPagingListAsync(selector: x => new GetConfigResponse(x.PakageName, x.PakageFee
                 ,x.Description ,x.DateCreate, x.IsActive), page: page, size: size, orderBy: x => x.OrderBy(x => x.PakageFee));
            var ConfigResponses = new Paginate<GetConfigResponse>()
            {
                Page = config.Page,
                Size = config.Size,
                Total = config.Total,
                TotalPages = config.TotalPages,
                Items = _mapper.Map<IList<GetConfigResponse>>(config.Items),
            };
            return ConfigResponses;            
        }

        public async Task<GetConfigResponse>? GetConfigbyIdAsync(Guid id)
        {
            GetConfigResponse configresponse = await _unitofwork.GetRepository<Config>().
                SingleOrDefaultAsync(selector: x => new GetConfigResponse(x.PakageName, x.PakageFee
                 , x.Description, x.DateCreate, x.IsActive), predicate: x => x.Id.Equals(id));

            if (configresponse == null) return null;
            return _mapper.Map<GetConfigResponse>(configresponse);
        }
 

        public async Task<bool> UpdateConfigAsync(Guid id, UpdateConfigRequest request)
        {
            
            var existConfig = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(
            predicate: e => e.Id == id,
            orderBy: null,
            include: null);

            if (existConfig == null)
            {
                throw new KeyNotFoundException("Cannot Find Config");
            }
            existConfig = _mapper.Map<Config>(request);
            _unitofwork.GetRepository<Config>().UpdateAsync(existConfig);
            bool isUpdate = await _unitofwork.CommitAsync() > 0;
            return isUpdate;

        }

        
    }
}
