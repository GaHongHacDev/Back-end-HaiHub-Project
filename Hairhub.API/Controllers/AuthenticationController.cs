using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Authentication;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Exceptions;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Hairhub.API.Constants.ApiEndPointConstant;


namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Authentication.AuthenticationEndpoint + "/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private IAuthenticationService _authenticationService;

        public AuthenticationController(IMapper _mapper, IAuthenticationService authenticationService) : base(_mapper)
        {
            _authenticationService = authenticationService;
        }

        // POST api/user/login
        //[AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var loginResponse = await _authenticationService.Login(loginRequest.Username, loginRequest.Password);

                if (loginResponse == null || String.IsNullOrWhiteSpace(loginResponse.ToString()))
                    return Unauthorized(new { message = "Tài khoản hoặc mật khẩu không đúng" });
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var refreshTokenResponse = await _authenticationService.RefreshToken(request);
                return Ok(refreshTokenResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> LogOut(LogoutRequest logoutRequest)
        {
            try
            {
                bool isLogout = await _authenticationService.Logout(logoutRequest);
                if (!isLogout)
                {
                    return BadRequest("Cannot logout account!");
                }
                return Ok("Logout successfully!");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Route("{accessToken}")]
        public async Task<IActionResult> FetchUser( string accessToken)
        {
            try
            {
                var response = await _authenticationService.FetchUser(accessToken);
                return Ok(response);
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
