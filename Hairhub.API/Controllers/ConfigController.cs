using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

            var listconfig = await _configservice.GetConfigAsync(page, size);
            return Ok(listconfig);
        }
        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetConfigById([FromRoute] Guid id)
        {
            try
            {
                var config = await _configservice.GetConfigbyIdAsync(id);
                if (config == null)
                {
                    return BadRequest("Cannont find this config!");
                }
                return Ok(config);
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
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateConfig([FromRoute] Guid id, [FromBody]UpdateConfigRequest request)
        {
            try
            {
                if(id == null)
                {
                    return BadRequest("Config Id is null or empty");
                }
                bool isUpdate = await _configservice.UpdateConfigAsync(id, request);
                if (!isUpdate)
                {
                    return BadRequest(new { message = "Không thể cập nhật gói" });
                }
                return Ok("Update ServiceHair successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteConfig([FromRoute]Guid id)
        {
            try
            {
                var isDelete = await _configservice.DeleteConfigAsync(id);
                if (!isDelete)
                {
                    return BadRequest("Không thể xóa gói");
                }
                return Ok("Deleted successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        
    }
}
