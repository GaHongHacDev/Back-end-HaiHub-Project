using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Common.ThirdParties.Contract;
using Hairhub.Common.ThirdParties.Implementation;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.AI;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Account.AccountsEndpoint + "/[action]")]
    [ApiController]
    public class AIController : BaseController
    {
        private IGeminiAIService _geminiAIService;
        public AIController(IMapper mapper, IGeminiAIService geminiAIService) : base(mapper)
        {
            _geminiAIService = geminiAIService;
        }

        [HttpPost]
        public async Task<IActionResult> ChatMessageAI([FromBody] AIChatMessageRequest chatMessageRequest)
        {
            try
            {
                var result = await _geminiAIService.ChatMessage(chatMessageRequest);
                if (chatMessageRequest == null)
                {
                    return BadRequest(new { message = "Lỗi trong quá trình chat với AI" });
                }
                return Ok(result);
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
