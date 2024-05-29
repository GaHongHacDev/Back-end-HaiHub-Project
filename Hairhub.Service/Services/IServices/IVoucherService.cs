using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IVoucherService
    {
        Task<IPaginate<GetVoucherResponse>> GetAllVoucherAsync(int page, int size);

        Task<GetVoucherResponse>? GetVoucherbyCodeAsync(string code);

        Task<GetVoucherResponse>? GetVoucherbyIdAsync(Guid id);

        Task<CreateVoucherResponse> CreateVoucherAsync(CreateVoucherRequest request);

        Task<UpdateVoucherResponse> UpdateVoucherAsync(Guid id,UpdateVoucherRequest request);

        Task DeleteVoucherAsync(Guid id);
    }
}
