using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Reports;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Report.ReportsEndpoint + "/[action]")]
    [ApiController]
    public class ReportController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportController(IMapper mapper, IReportService reportService) : base(mapper)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllReport([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var reportsResponse = await _reportService.GetAllReport(page, size);
            return Ok(reportsResponse);
        }

        [HttpGet]
        [Route("{roleName}")]
        public async Task<IActionResult> GetAllReportByRoleName([FromRoute] string roleName, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var reportsResponse = await _reportService.GetAllReportByRoleName(roleName, page, size);
            return Ok(reportsResponse);
        }

        [HttpGet]
        [Route("{customerId:Guid}")]
        public async Task<IActionResult> GetReportByCustomerId([FromRoute] Guid customerId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var reportsResponse = await _reportService.GetReportByCustomerId(customerId, page, size);
            return Ok(reportsResponse);
        }

        [HttpGet]
        [Route("{salonId:Guid}")]
        public async Task<IActionResult> GetReportBySalonId([FromRoute] Guid salonId, [FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var reportsResponse = await _reportService.GetReportBySalonId(salonId, page, size);
            return Ok(reportsResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromForm] CreateReportRequest createReportRequest)
        {
            try
            {
                var reportResponse = await _reportService.CreateReport(createReportRequest);
                if (reportResponse == false)
                {
                    return NotFound(new { message = "Không thể tạo đơn báo cáo" });
                }
                return Ok("Tạo báo cáo thành công");
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> ConfirmReport([FromRoute] Guid id, [FromBody] ConfirmReportRequest confirmReportRequest)
        {
            try
            {
                bool isConfirm = await _reportService.ConfirmReport(id, confirmReportRequest);
                if (!isConfirm)
                {
                    return BadRequest(new { message = "Không thể duyệt đơn báo cáo" });
                }
                return Ok("Duyệt đơn đặt báo cáo thành công");
            }
            catch (NotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
