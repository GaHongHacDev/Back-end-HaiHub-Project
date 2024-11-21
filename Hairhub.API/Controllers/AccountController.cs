using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Hairhub.Service.Services.Services;
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
        public async Task<IActionResult> RegisterAccount([FromBody] CreateAccountRequest createAccountRequest)
        {
            try
            {
                var accoutResponse = await _accountService.RegisterAccount(createAccountRequest);
                if (accoutResponse == null)
                {
                    return BadRequest("Cannot register account!");
                }
                return Ok(accoutResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> UpdateAccount([FromRoute] Guid id, [FromForm] UpdateAccountRequest updateAccountRequest)
        {
            try
            {
                if (id == null)
                {
                    return BadRequest("Account Id is null or empty!");
                }

                var updateAccountResponse = await _accountService.UpdateAccountById(id, updateAccountRequest);
                if (updateAccountResponse == null)
                {
                    return BadRequest("Cannot update account");
                }
                return Ok(updateAccountResponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
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
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> ChangePassword([FromRoute] Guid id, [FromBody] ChangePasswordRequest changePasswordRequest)
        {
            try
            {
                var isChangePassword = await _accountService.ChangePassword(id, changePasswordRequest);
                if (!isChangePassword)
                {
                    return BadRequest("Cannot change password!");
                }
                return Ok("Change password successfully!");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> DeleteAccount([FromRoute] Guid id)
        {
            try
            {
                var isDelete = await _accountService.DeleteAccountById(id);
                if (!isDelete)
                {
                    return BadRequest("Cannot delete this account!");
                }
                return Ok("Delete account successfully!");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetAccountById([FromRoute] Guid id)
        {
            try
            {
                var accooutReponse = await _accountService.GetAccountById(id);
                if (accooutReponse==null)
                {
                    return BadRequest("Cannot find this account!");
                }
                return Ok(accooutReponse);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
