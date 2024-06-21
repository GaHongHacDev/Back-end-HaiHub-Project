using Hairhub.Domain.Dtos.Requests.Approval;
using Hairhub.Domain.Dtos.Responses.Approval;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IApprovalService
    {
        Task<IPaginate<GetApprovalResponse>> GetApprovals(int page, int size);
        Task<List<GetApprovalResponse>> GetSalonApprovals(Guid salonId);
        Task<GetApprovalResponse> GetApprovalById(Guid id);
        Task<bool> CreateApproval(CreateApprovalRequest request);
        Task<bool> UpdateApproval(Guid id, UpdateApprovalRequest request);
        Task<bool> DeleteApproval(Guid id);
    }
}
