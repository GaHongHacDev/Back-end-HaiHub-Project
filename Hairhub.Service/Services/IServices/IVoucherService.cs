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
        Task<IPaginate<GetVoucherResponse>> GetVoucherAsync(int page, int size);

        Task<GetVoucherResponse>? GetVoucherbyCodeAsync(string code);

        Task<GetVoucherResponse>? GetVoucherbyIdAsync(Guid id);

        Task<bool> CreateVoucherAsync(CreateVoucherRequest request);

        Task<bool> UpdateVoucherAsync(Guid id,UpdateVoucherRequest request);

        Task<bool> DeleteVoucherAsync(Guid id);
    }
}
