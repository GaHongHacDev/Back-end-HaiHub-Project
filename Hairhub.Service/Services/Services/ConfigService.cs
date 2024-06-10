using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Responses.Config;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;
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
                DateCreate = request.DateCreate,
                IsActive = request.IsActive,
            };

            await _unitofwork.GetRepository<Config>().InsertAsync(config);
            await _unitofwork.CommitAsync();
            return _mapper.Map<CreateConfigResponse>(config);
        }

        

        public async Task DeleteConfigAsync(Guid id)
        {
            var existconfig = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(predicate: e => e.Id == id,
               orderBy: null,
               include: null);
            if (existconfig == null)
            {
                throw new Exception("Voucher Not Found");
            }

            _unitofwork.GetRepository<Config>().DeleteAsync(existconfig);
            _unitofwork.Commit();

        }

 /*       public async Task<IPaginate<GetConfigResponse>> GetAllConfigAsync(int page, int size)
        {
            IPaginate<GetConfigResponse> config = await _unitofwork.GetRepository<Config>()
                 .GetPagingListAsync(selector: x => new GetConfigResponse(x.PakageName, x.PakageFee
                 , x.DateCreate, x.IsActive), page: page, size: size, orderBy: x => x.OrderBy(x => x.DateCreate));

            return config;
        }

        public async Task<GetConfigResponse>? GetConfigbyIdAsync(Guid id)
        {
            GetConfigResponse configresponse = await _unitofwork.GetRepository<Config>().
                SingleOrDefaultAsync(selector: x => new GetConfigResponse(x.CommissionRate, x.MaintenanceFee,
                x.DateCreate,x.IsActive), predicate: x => x.Id.Equals(id));

            if (configresponse == null) return null;
            return configresponse;
        }
 */

        public async Task<UpdateConfigResponse> UpdateConfigAsync(Guid id, UpdateConfigRequest request)
        {
            
            var existConfig = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(
            predicate: e => e.Id == id,
            orderBy: null,
            include: null);

            if (existConfig == null)
            {
                throw new KeyNotFoundException("Cannot Find Config");
            }

            existConfig.PakageName = request.PakageName;
            existConfig.PakageFee = request.PakageFee;
            existConfig.Description = request.Description;
            existConfig.DateCreate = request.DateCreate;
            existConfig.IsActive = request.IsActive;

            _unitofwork.GetRepository<Config>().UpdateAsync(existConfig);
            _unitofwork.Commit();
            return _mapper.Map<UpdateConfigResponse>(existConfig);
            
        }

        
    }
}
