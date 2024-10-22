using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Account.AccountsEndpoint + "/[action]")]
    [ApiController]
    public class AccountController : BaseController
    {
        private IAccountService _accountService;
        public AccountController(IMapper mapper, IAccountService accountService) : base(mapper)
        {
            _accountService = accountService;
        }

        [HttpPost]
        public async Task<IActionResult> GoogleLogin(CheckLoginGoogleRequest checkLoginGoogle)
        {
            try
            {
                var payload = await _accountService.LoginGoogle(checkLoginGoogle);
                return Ok(payload);
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

        [HttpPost]
        public async Task<IActionResult> RegisterAccount([FromBody] CreateAccountRequest createAccountRequest)
        {
            try
            {
                var accoutResponse = await _accountService.RegisterAccount(createAccountRequest);
                if (accoutResponse == null)
                {
                    return BadRequest(new { message = "Không thể đăng ký tài khoản" });
                }
                return Ok(accoutResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccountLoginGoogle([FromBody] CreateAccountLoginGoogleRequest createAccountRequest)
        {
            try
            {
                var accoutResponse = await _accountService.RegisterAccountLoginGoogle(createAccountRequest);
                if (accoutResponse == null)
                {
                    return BadRequest(new { message = "Không thể đăng ký tài khoản" });
                }
                return Ok(accoutResponse);
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> UpdateAccount([FromRoute] Guid id, [FromForm] UpdateAccountRequest updateAccountRequest)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("Account Id is null or empty!");
                }

                bool updateSuccess = await _accountService.UpdateAccountById(id, updateAccountRequest);
                if (updateSuccess == false)
                {
                    return BadRequest(new { massage = "Cập nhật tài khoản thất bại" });
                }
                return Ok("Cập nhật tài khoản thành công");
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
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> ActiveAccount([FromRoute] Guid id)
        {
            try
            {
                var isDelete = await _accountService.ActiveAccount(id);
                if (!isDelete)
                {
                    return BadRequest("Cannot delete this account!");
                }
                return Ok("Active account successfully!");
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
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                var isChangePassword = await _accountService.ChangePassword(id, changePasswordRequest);
                if (!isChangePassword)
                {
                    return BadRequest(new { message = "Không thể đổi mật khẩu" });
                }
                return Ok("Đổi mật khẩu thành công");
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

        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.Customer)]
        public async Task<IActionResult> DeleteAccount([FromRoute] Guid id)
        {
            try
            {
                var isDelete = await _accountService.DeleteAccountById(id);
                if (!isDelete)
                {
                    return BadRequest(new { message = "Không thể xóa tài khoản" });
                }
                return Ok("Xóa tài khoản thành công");
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

        [HttpGet]
        [Route("{id:Guid}")]
        [Authorize(Roles = RoleNameAuthor.Admin + "," + RoleNameAuthor.SalonOwner + "," + RoleNameAuthor.Customer + "," + RoleNameAuthor.SalonEmployee)]
        public async Task<IActionResult> GetAccountById([FromRoute] Guid id)
        {
            try
            {
                var accooutReponse = await _accountService.GetAccountById(id);
                if (accooutReponse==null)
                {
                    return BadRequest(new { message = "Không tìm thấy tài khoản" });
                }
                return Ok(accooutReponse);
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

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> GetNumberofAccountinYear([FromQuery]int? year)
        {
            try
            {
                var accooutReponse = await _accountService.GetNumberOfCustomerOnMonth(year);
                if (accooutReponse == null)
                {
                    return BadRequest(new { message = "Không tìm thấy tài khoản" });
                }
                return Ok(accooutReponse);
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
        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> GetNumberofSalonOwnerinYear([FromQuery]int? year)
        {
            try
            {
                var accooutReponse = await _accountService.GetNumberOfSalonOwnerOnMonth(year);
                if (accooutReponse == null)
                {
                    return BadRequest(new { message = "Không tìm thấy tài khoản" });
                }
                return Ok(accooutReponse);
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

        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> GetNumberofCustomerApproved()
        {
            try
            {
                var accooutReponse = await _accountService.GetCustomersActive();
                if (accooutReponse == null)
                {
                    return BadRequest(new { message = "Không tìm thấy tài khoản" });
                }
                return Ok(accooutReponse);
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
        [HttpGet]
        [Authorize(Roles = RoleNameAuthor.Admin)]
        public async Task<IActionResult> GetNumberofSalonOwnerApproved()
        {
            try
            {
                var accooutReponse = await _accountService.GetSalonsActive();
                if (accooutReponse == null)
                {
                    return BadRequest(new { message = "Không tìm thấy tài khoản" });
                }
                return Ok(accooutReponse);
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


        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            try
            {
                var result = await _accountService.ForgotPassword(request);
                if (result == null)
                {
                    return BadRequest(new { message = "Không tồn tại Email này" });
                }
                return Ok(new { success = result, message = "Lấy Password thành công"});
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
