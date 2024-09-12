using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Dtos.Responses.SalonInformations;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface ISalonInformationService
    {
        Task<List<SalonSuggesstionResponse>> GetSalonSuggestion();
        Task<IPaginate<GetSalonInformationResponse>> GetAllApprovedSalonInformation(int page, int size);
        Task<List<GetSalonInformationResponse>> GetAllApprovedSalonInformationNoPaging();
        Task<IPaginate<GetSalonInformationResponse>> GetAllSalonByAdmin(int page, int size);
        Task<IPaginate<GetSalonInformationResponse>> GetSalonByStatus(string? name, string? status, int page, int size);
        Task<GetSalonInformationResponse>? GetSalonInformationById(Guid id);
        Task<CreateSalonInformationResponse> CreateSalonInformation(CreateSalonInformationRequest createSalonInformationRequest);
        Task<bool> UpdateSalonInformationById(Guid id, UpdateSalonInformationRequest updateSalonInformationRequest);
        Task<bool> DeleteSalonInformationById(Guid id);
        Task<bool> ActiveSalonInformation(Guid id);
        Task<GetSalonInformationResponse>? GetSalonByOwnerId(Guid ownerId);
        Task<IPaginate<SearchSalonByNameAddressServiceResponse>> SearchSalonByNameAddressService(int page, int size, string? serviceName = "",
                                        string? salonAddress = "", string? salonName = "", decimal? latitude = 0, decimal? longtitude = 0, decimal? distance = 0);
    }
}
