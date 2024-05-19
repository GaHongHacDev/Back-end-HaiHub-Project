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
    }
}
