using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
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
    public interface IAppointmentDetailService
    {
        Task<IPaginate<GetAppointmentDetailResponse>> GetAllAppointmentDetail(int page, int size);
        Task<GetAppointmentDetailResponse>? GetAppointmentDetailById(Guid id);
        Task<CreateAppointmentDetailResponse> CreateAppointmentDetail(CreateAppointmentDetailRequest createAppointmentDetailRequest);
        Task<CreateAppointmentDetailResponse> CreateAppointmentDetailFromAppointment(Guid appointmentId, AppointmentDetailRequest createAppointmentDetailRequest);
        Task<bool> UpdateAppointmentDetailById(Guid id, UpdateAppointmentDetailRequest updateAppointmentDetailRequest);
        Task<bool> DeleteAppoinmentDetailById(Guid id);
    }
}
