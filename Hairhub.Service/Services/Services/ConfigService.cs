﻿using AutoMapper;
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
            try
            {
                var config = new Config()
                {
                    PakageName = request.PakageName,
                    PakageFee = request.PakageFee,
                    Description = request.Description,
                    NumberOfDay = request.NumberOfDay,
                    DateCreate = DateTime.Now,
                    IsActive = request.IsActive,
                };

                await _unitofwork.GetRepository<Config>().InsertAsync(config);
                await _unitofwork.CommitAsync();
                return _mapper.Map<CreateConfigResponse>(config);

            } catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        

        public async Task<bool> DeleteConfigAsync(Guid id)
        {
            var existconfig = await _unitofwork.GetRepository<Config>().SingleOrDefaultAsync(predicate: e => e.Id == id);
            if (existconfig == null)
            {
                throw new NotFoundException("ServiceHair not found!");
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
                throw new KeyNotFoundException("Cannot Find Config");
            }
            existConfig = _mapper.Map<Config>(request);
            _unitofwork.GetRepository<Config>().UpdateAsync(existConfig);
            bool isUpdate = await _unitofwork.CommitAsync() > 0;
            return isUpdate;
        }

        
    }
}
