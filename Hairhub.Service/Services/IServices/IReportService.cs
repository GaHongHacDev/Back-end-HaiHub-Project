using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Reports;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Reports;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IReportService
    {
        Task<IPaginate<GetReportResponse>> GetAllReport(int page, int size);
        Task<IPaginate<GetReportResponse>> GetReportByCustomerId(Guid customerId, int page, int size);
        Task<IPaginate<GetReportResponse>> GetReportBySalonId(Guid salonId, int page, int size);
        Task<bool> CreateReport(CreateReportRequest request);
        Task<bool> ConfirmReport(Guid id, ConfirmReportRequest request);
        Task<IPaginate<GetReportResponse>> GetAllReportByRoleName(string roleNameReport, int page, int size);
    }
}
