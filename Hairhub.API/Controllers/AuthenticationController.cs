using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.Domain.Dtos.Requests.Accounts;
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
            var token = await _authenticationService.Login(loginRequest.Username, loginRequest.Password);

            if (token == null || String.IsNullOrWhiteSpace(token.ToString()))
                return BadRequest(new { message = "User name or password is incorrect" });
            return Ok(token);
        }
    }
}
