using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Config.ConfigEndpoint + "/[action]")]
    [ApiController]
    public class ConfigController : BaseController
    {
        private readonly IConfigService _configservice;

        public ConfigController(IMapper mapper, IConfigService configservice) : base(mapper)
        {
            _configservice = configservice;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllConfig([FromQuery] int page=1, [FromQuery] int size = 10)
        {

            var listconfig = await _configservice.GetAllConfigAsync(page, size);
            return Ok(listconfig);
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetConfigById([FromRoute] Guid id)
        {
            try
            {
                var Config = await _configservice.GetConfigbyIdAsync(id);
                if (Config == null)
                {
                    return NotFound("Cannont find this config!");
                }
                return Ok(Config);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        


        [HttpPost]
        public async Task<IActionResult> CreateConfig([FromBody]CreateConfigRequest request)
        {
            try
            {
                var result = await _configservice.CreateConfigAsync(request);
                if (result == null)
                {
                    return NotFound("Cannot create config!!!");
                }
                return Ok(result);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConfig([FromRoute] Guid id, [FromBody]UpdateConfigRequest request)
        {
            try
            {
                var result = await _configservice.UpdateConfigAsync(id, request);
                if (result == null)
                {
                    return NotFound("Cannot Update Config");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteConfig([FromRoute] Guid id)
        {
            try
            {
                await _configservice.DeleteConfigAsync(id);
                return Ok(new { Message = "Config deleted successfully." });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}
