using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
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
        Task<IPaginate<GetAppointmentResponse>> GetAllAppointment(int page, int size);
        Task<GetAppointmentResponse>? GetAppointmentById(Guid id);
        Task<CreateAppointmentResponse> CreateAppointment(CreateAppointmentRequest createAccountRequest);
        Task<bool> UpdateAppointmentById(Guid id, UpdateAppointmentRequest updateAppointmentRequest);
        Task<bool> DeleteAppoinmentById(Guid id);
        Task<bool> ActiveAppointment(Guid id);
        Task<GetAvailableTimeResponse> GetAvailableTime(GetAvailableTimeRequest getAvailableTimeRequest);
    }
}
