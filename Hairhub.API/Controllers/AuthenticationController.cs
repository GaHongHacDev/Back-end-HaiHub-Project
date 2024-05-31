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
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var loginResponse = await _authenticationService.Login(loginRequest.Username, loginRequest.Password);

                if (loginResponse == null || String.IsNullOrWhiteSpace(loginResponse.ToString()))
                    return Unauthorized(new { message = "User name or password is incorrect" });
                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult LogOut()
        {
            // Đọc token từ header X-Auth-Token
            var token = Request.Headers["X-Auth-Token"].FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Token is missing");
            }

            _authenticationService.Logout(token);

            return Ok();
        }
    }
}
