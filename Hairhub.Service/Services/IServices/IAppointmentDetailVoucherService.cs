using Hairhub.Domain.Dtos.Requests.AppointmentDetailVouchers;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IAppointmentDetailVoucherService
    {
        //Task<IPaginate<GetAppointmentDetailVoucherResponse>> GetAppointmentDetailVouchers(int page, int size);
        //Task<GetAppointmentDetailVoucherResponse> GetAppointmentDetailVoucherById(Guid id);
        Task<bool> CreateAppointmentDetailVoucher(CreateAppointmentDetailVoucherRequest request);
        Task<bool> UpdateAppointmentDetailVoucher(Guid id, UpdateAppointmentDetailVoucherRequest request);
    }
}
