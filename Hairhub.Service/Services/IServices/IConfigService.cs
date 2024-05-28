using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Domain.Dtos.Responses.Config;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IConfigService
    {
        Task<IPaginate<GetConfigResponse>> GetAllConfigAsync(int page, int size);


        Task<GetConfigResponse>? GetConfigbyIdAsync(Guid id);

        Task<CreateConfigResponse> CreateConfigAsync(CreateConfigRequest request);

        Task<UpdateConfigResponse> UpdateConfigAsync(Guid id, UpdateConfigRequest request);

        Task DeleteConfigAsync(Guid id);
    }
}
