using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
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
    public class AppointmentDetailService : IAppointmentDetailService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentDetailService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IPaginate<AppointmentDetailResponse>> GetAllAppointmentDetail(int page, int size)
        {
            IPaginate<AppointmentDetailResponse> appointmentDetailResponse = await _unitOfWork.GetRepository<AppointmentDetail>()
            .GetPagingListAsync(
              selector: x => new AppointmentDetailResponse(x.Id, x.SalonEmployeeId, x.ServiceHairId, x.AppointmentId, x.Description,
                                                     x.Date, x.Time, x.OriginalPrice, x.DiscountedPrice, x.Status),
              page: page,
              size: size,
              orderBy: x => x.OrderBy(x => x.Date));
            return appointmentDetailResponse;
        }
    }
}
