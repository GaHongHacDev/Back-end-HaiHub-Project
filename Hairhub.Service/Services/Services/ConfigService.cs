using AutoMapper;
using CloudinaryDotNet.Actions;
using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Responses.Config;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Math.EC.Rfc7748;
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

        public async Task<bool> CreateCommisionConfig(CreateCommisionConfigRequest request)
        {
            if (!request.Type.Equals(ConfigType.Commission))
            {
                throw new NotFoundException("Loại gói không hợp lệ");
            }
            var config = new Config()
            {
                Id = Guid.NewGuid(),
                PakageName = request.PakageName,
                Description = request.Description,
                CommissionRate = request.CommissionRate,
                DateCreate = DateTime.Now,
                IsActive = request.IsActive,
                Type = request.Type,
            };
            await _unitofwork.GetRepository<Config>().InsertAsync(config);
            bool isCreate = await _unitofwork.CommitAsync()>0;
            return isCreate;
        }
        public async Task<bool> CreateSubcriptionConfig(CreateSubscriptionConfigRequest request)
        {
            if (!request.Type.Equals(ConfigType.Subcription))
            {
                throw new NotFoundException("Loại gói không hợp lệ");
            }
            var config = new Config()
            {
                Id = Guid.NewGuid(),
                PakageName = request.PakageName,
                Description = request.Description,
                PakageFee = request.PakageFee,
                DateCreate = DateTime.Now,
                NumberOfDay = request.NumberOfDay,
                IsActive = request.IsActive,
                Type = request.Type,
            };
            await _unitofwork.GetRepository<Config>().InsertAsync(config);
            bool isCreate = await _unitofwork.CommitAsync() > 0;
            return isCreate;
        }

        public async Task<bool> DeleteConfigAsync(Guid id)
        {
            var existconfig = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(predicate: e => e.Id == id);
            if (existconfig == null)
            {
                throw new NotFoundException("Không tìm thấy Config này ");
            }
            _unitofwork.GetRepository<Config>().DeleteAsync(existconfig);
            bool isUpdate = await _unitofwork.CommitAsync() > 0;
            return isUpdate;

        }

        public async Task<IPaginate<GetConfigResponse>> GetConfigs(int page, int size)
        {
            var config = await _unitofwork.GetRepository<Config>().GetPagingListAsync(page: page, size: size);
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
            var config = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.Id == id);
            if (config == null)
            {
                throw new NotFoundException("Không tìm thấy Config này ");
            }
            return _mapper.Map<GetConfigResponse>(config);
        }


        public async Task<bool> UpdateConfigAsync(Guid id, UpdateConfigRequest request)
        {

            var existConfig = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(
            predicate: e => e.Id == id);

            if (existConfig == null)
            {
                throw new NotFoundException("Không tìm thấy Config này ");
            }
            if (existConfig.Type.Equals(ConfigType.Commission))
            {
                existConfig.PakageName = request.PakageName;
                existConfig.Description = request.Description;
                existConfig.CommissionRate = request.CommissionRate;
                existConfig.IsActive = request.IsActive;
            }
            else
            {
                existConfig.PakageName = request.PakageName;
                existConfig.Description = request.Description;
                existConfig.IsActive = request.IsActive;
                existConfig.NumberOfDay = request.NumberOfDay;
                existConfig.PakageFee = request.PakageFee;
            }
            _unitofwork.GetRepository<Config>().UpdateAsync(existConfig);
            bool isUpdate = await _unitofwork.CommitAsync() > 0;
            return isUpdate;
        }

        public async Task<Guid> GetConfigIdofCommissionRate()
        {
            var config = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(predicate: p => p.CommissionRate != null && p.Type.Equals(ConfigType.Commission));
            Guid id = config.Id;
            return id;
        }

        public async Task<IPaginate<GetConfigResponse>> GetConfigByType(string? type, int page, int size)
        {
            if (type.IsNullOrEmpty())
            {
                type = "";
            }
            var config = await _unitofwork.GetRepository<Config>().GetPagingListAsync(predicate: x=>x.Type.Contains(type!) && x.IsActive, page: page, size: size);
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
    }
}
