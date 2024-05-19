using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
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
        Task<IPaginate<AppointmentDetailResponse>> GetAllAppointmentDetail(int page, int size);
    }
}
