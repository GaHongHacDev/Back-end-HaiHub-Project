using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IPaginate<GetAppointmentResponse>> GetAllAppointment(int page, int size)
        {
            IPaginate<GetAppointmentResponse> appointmentsResponse = await _unitOfWork.GetRepository<Appointment>()
            .GetPagingListAsync(
              selector: x => new GetAppointmentResponse(x.Id, x.Date, x.TotalPrice, x.CustomerId, x.IsActive),
              page: page,
              size: size,
              orderBy: x => x.OrderBy(x => x.Date));
            return appointmentsResponse;
        }

        public async Task<GetAppointmentResponse>? GetAppointmentById(Guid id)
        {
            GetAppointmentResponse appointmentResponse = await _unitOfWork
                .GetRepository<Appointment>()
                .SingleOrDefaultAsync(
                    selector: x => new GetAppointmentResponse(x.Id, x.Date, x.TotalPrice, x.CustomerId, x.IsActive),
                    predicate: x => x.Id.Equals(id)
                 );
            if (appointmentResponse == null) 
                return null;
            return appointmentResponse;
        }
    }
}
