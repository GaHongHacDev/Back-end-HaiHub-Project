using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Dtos.Responses.Dashboard;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IAppointmentService
    {
        //Get
        Task<IPaginate<GetAppointmentResponse>> GetAllAppointment(int page, int size);
        Task<GetAppointmentResponse>? GetAppointmentById(Guid id);
        Task<List<GetAppointmentResponse>> GetAppointmentSalonBySalonIdNoPaging(Guid salonId);
        Task<GetAvailableTimeResponse> GetAvailableTime(GetAvailableTimeRequest getAvailableTimeRequest);
        Task<IPaginate<GetAppointmentResponse>> GetAppointmentByAccountId(Guid AccountId, int page, int size);
        Task<IPaginate<GetAppointmentResponse>> GetHistoryAppointmentByCustomerId(int page, int size, Guid CustomerId);
        Task<IPaginate<GetAppointmentResponse>> GetBookingAppointmentByCustomerId(int page, int size, Guid CustomerId);
        Task<IPaginate<GetAppointmentResponse>> GetAppointmentSalonByStatus(int page, int size, Guid SalonId, string? Status);
        Task<IPaginate<GetAppointmentResponse>> GetAppointmentEmployeeByStatus(int page, int size, Guid EmployeeId, string? Status);
        Task<List<GetAppointmentResponse>> GetAppointmentSalonByStatusNoPaing(Guid salonId, string? status, DateTime? startDate, DateTime? endDate);
        Task<GetAppointmentTransactionResponse> GetAppointmentTransaction(Guid salonId, int NumberOfDay);

        Task<GetCalculatePriceResponse> CalculatePrice(GetCalculatePriceRequest calculatePriceRequest);
        Task<BookAppointmentResponse> BookAppointment(BookAppointmentRequest request);
        Task<bool> CreateAppointment(CreateAppointmentRequest request);
        Task<bool> UpdateAppointmentById(Guid id, UpdateAppointmentRequest updateAppointmentRequest);
        Task<bool> CancelAppointmentByCustomer(Guid id, CancelApointmentRequest cancelApointmentRequest);
        Task<bool> DeleteAppoinmentById(Guid id);
        Task<bool> ActiveAppointment(Guid id);

        Task<DataOfMonths> GetAppointmentbyStatusByAdmin(string status, int year);
        Task<DataOfMonths> GetRevenueByAdmin(int year);

        Task<DataOfMonths> GetCommissionByAdmin(int year);

        Task<List<RatioData>> GetPercentagebyStatusOfAppointmentByAdmin(int? year);

        Task<List<MonthlyRatioData>> GetPercentageOfAppointmentByAdmin(int? year);

    }
}

