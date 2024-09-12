using Hairhub.Domain.Dtos.Requests.SalonOwners;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.SalonOwners;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface ISalonOwnerService
    {
        Task<IPaginate<GetSalonOwnerResponse>> GetAllSalonOwner(string? email, bool? status, int page, int size);
        Task<GetSalonOwnerResponse>? GetSalonOwnerById(Guid id);
        Task<CreateSalonOwnerResponse> CreateSalonOwner(CreateSalonOwnerRequest createAccountRequest);
        Task<bool> UpdateSalonOwnerById(Guid id, UpdateSalonOwnerRequest updateSalonOwnerRequest);
        Task<bool> DeleteSalonOwnerById(Guid id);
    }
}
