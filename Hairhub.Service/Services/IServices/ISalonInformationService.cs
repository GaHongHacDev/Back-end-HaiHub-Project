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
        Task<GetSalonInformationResponse>? GetSalonByEmployeeId(Guid id);

        Task<IPaginate<ServiceStatistics>> ServiceStatistics(Guid salonId, DateTime? startDate, DateTime? endDate, string? filter, int page, int size);
        Task<RevenueStatistics> RevenueStatistics(Guid salonId, DateTime? startDate, DateTime? endDate);
        Task<RevenueByHours> RevenueStatistics(Guid salonId, DateTime? Date);

        Task<CreateSalonInformationResponse> CreateSalonInformation(CreateSalonInformationRequest createSalonInformationRequest);
        Task<bool> UpdateSalonInformationById(Guid id, UpdateSalonInformationRequest updateSalonInformationRequest);
        Task<bool> DeleteSalonInformationById(Guid id);
        Task<bool> ActiveSalonInformation(Guid id);
        Task<GetSalonInformationResponse>? GetSalonByOwnerId(Guid ownerId);
        Task<IPaginate<SearchSalonByNameAddressServiceResponse>> SearchSalonByNameAddressService(int page, int size, string? serviceName = "",
                                        string? salonAddress = "", string? salonName = "", decimal? latitude = 0, decimal? longtitude = 0, decimal? distance = 0);
        Task<ReviewRevenueReponse> ReviewRevenue(Guid SalonId, DateTime startDate, DateTime endDate);

        Task<bool> AddSalonInformationImages(Guid Salonid, AddSalonImagesRequest request);

        Task<IPaginate<SalonInformationImagesResponse>> GetSalonInformationImages(Guid salonid, int page, int size);

        Task<bool> DeleteSalonInformationImages(DeleteImagesRequest request);

        Task<CompileRevenueSalonByYearResponse> CompileRevenueSalonByYear(Guid salonId, int year);
        Task<CompileAppointmentSalonByYearResponse> CompileAppointmentSalonByYear(Guid salonId, int year);
        Task<IPaginate<EmployeeStatictisResponse>> CompileEmployeeRevenue(Guid salonId, DateTime startDate, DateTime endDate, string? filter, int page, int size);

        Task<List<CompileAppointmentSalonResponse>> CompileAppointmentSalon(Guid salonId, DateTime startDate, DateTime endDate);


        Task<StatisticsOfSalonsParticipating> StatisticsOfSalonsParticipating(string filter);

        Task<StatisticsOfSalonsParticipating> StatisticsOfAccountParticipating(string filter);

        Task<StatisticsOfSalonsParticipating> StatisticsOfSalonsRevenue(Guid? salonid, string? filter);

        Task<StatisticsOfSalonsParticipating> StatisticsRevenueOfPlatForm(string? filter);
        Task<GetCustomerQuantityResponse> CustomerQuantityToday(Guid salonId, DateTime date);

        Task<List<GetServiceStatisticByDateResponse>> GetServiceStatisticByDate(Guid salonId, DateTime date);
    }
}
