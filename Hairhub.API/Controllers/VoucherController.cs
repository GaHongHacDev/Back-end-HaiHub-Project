using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Voucher.VoucherEndpoint + "/[action]")]
    [ApiController]
    public class VoucherController : BaseController
    {
        private readonly IVoucherService _voucherService;

        public VoucherController(IMapper mapper, IVoucherService voucherService) : base(mapper)
        {
            _voucherService = voucherService;
        }
        

        [HttpGet]
        public async Task<IActionResult> GetAllVoucher([FromQuery]int page=1, [FromQuery]int size=10) {
            
            var listVoucher = await _voucherService.GetVoucherAsync(page, size);
            return Ok(listVoucher);        
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetVoucherById([FromRoute] Guid id)
        {
            try
            {
                var Voucher = await _voucherService.GetVoucherbyIdAsync(id);
                if (Voucher == null)
                {
                    return NotFound("Cannont find this voucher!");
                }
                return Ok(Voucher);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAdminVoucher([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            var listVoucher = await _voucherService.GetAdminVoucher(page, size);
            return Ok(listVoucher);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetVoucherBySalonId(
        [FromRoute] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int size = 10,
        [FromQuery] string search = null,
        [FromQuery] string filter = null,
        [FromQuery] string orderby = null)
        {
            try
            {
                var Voucher = await _voucherService.GetVoucherbySalonId(id, page, size, orderby, filter, search);
                if (Voucher == null)
                {
                    return NotFound("Cannont find this voucher!");
                }
                return Ok(Voucher);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetVoucherBySalonIdNoPaging([FromRoute] Guid id)
        {
            try
            {
                var Voucher = await _voucherService.GetVoucherbySalonId(id);
                return Ok(Voucher);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetVoucherByCode(string code)
        {
            try
            {
                var Voucher = await _voucherService.GetVoucherbyCodeAsync(code);
                if (Voucher == null)
                {
                    return NotFound("Cannot find this voucher!");
                }
                return Ok(Voucher);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody]CreateVoucherRequest request)
        {
            try
            {
                var result = await _voucherService.CreateVoucherAsync(request);
                if(result == false)
                {
                    return NotFound("Cannot create voucher!!!");
                }
                return Ok("Creted Successfully");

            }catch(NotFoundException ex)
            {
                return NotFound(new { message=ex.Message});
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateVoucher([FromRoute]Guid id, [FromBody]UpdateVoucherRequest request)
        {
            try
            {
                var result = await _voucherService.UpdateVoucherAsync(id, request);
                if(result == false)
                {
                    return NotFound("Cannot Update Voucher");
                }
                return Ok("Updated successfully");
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteVoucher([FromRoute]Guid id)
        {
            try
            {
                var result = await _voucherService.DeleteVoucherAsync(id);
                if (result == false)
                {
                    return NotFound("Cannot Delete Voucher");
                }
                return Ok("Deleted successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
