using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Approval;
using Hairhub.Domain.Dtos.Responses.Approval;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Approval.ApprovalsEndpoint + "/[action]")]
    [ApiController]
    public class ApprovalController : ControllerBase
    {
        private readonly IApprovalService _approvalService;
        private readonly IMapper _mapper;

        public ApprovalController(IApprovalService approvalService, IMapper mapper)
        {
            _approvalService = approvalService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IPaginate<GetApprovalResponse>>> GetApprovals(int page = 1, int size = 10)
        {
            var approvals = await _approvalService.GetApprovals(page, size);
            return Ok(approvals);
        }

        [HttpGet("salon/{salonId}")]
        public async Task<ActionResult<List<GetApprovalResponse>>> GetSalonApprovals(Guid salonId)
        {
            var approvals = await _approvalService.GetSalonApprovals(salonId);
            return Ok(approvals);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GetApprovalResponse>> GetApprovalById(Guid id)
        {
            var approval = await _approvalService.GetApprovalById(id);
            return Ok(approval);
        }

        [HttpPost]
        public async Task<ActionResult> CreateApproval([FromBody] CreateApprovalRequest request)
        {
            if (await _approvalService.CreateApproval(request))
            {
                return CreatedAtAction(nameof(GetApprovalById), new { id = request.SalonInformationId }, request);
            }
            return BadRequest("Failed to create approval");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateApproval(Guid id, [FromBody] UpdateApprovalRequest request)
        {
            if (await _approvalService.UpdateApproval(id, request))
            {
                return NoContent();
            }
            return BadRequest("Failed to update approval");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteApproval(Guid id)
        {
            if (await _approvalService.DeleteApproval(id))
            {
                return NoContent();
            }
            return NotFound("Approval not found");
        }
    }
}
