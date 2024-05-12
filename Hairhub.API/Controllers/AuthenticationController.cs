using Hairhub.API.Constants;
using Hairhub.API.Middlewares;
using Hairhub.Aplication.ICustomerServices;
using Hairhub.Domain.Payload.Request;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Hairhub.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    public class AuthenticationController : BaseController<AuthenticationController>
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IHostPartyService _hostPartyService;
        public AuthenticationController(ILogger<AuthenticationController> logger
            , IAuthenticationService authenticationService,
              IHttpContextAccessor httpContextAccessor, ICustomerService customerService
            , IHostPartyService hostPartyService)
            : base(logger)
        {
            _authenticationService = authenticationService;
            _httpContextAccessor = httpContextAccessor;
            _customerService = customerService;
            _hostPartyService = hostPartyService;
        }

        [HttpPost(ApiEndPointConstant.Authentication.Login)]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            var loginResponse = await _authenticationService.Login(loginRequest);
            if (loginResponse == null)
            {
                return Unauthorized(new ErrorResponse()
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Error = "Invalid or account is banned !",
                    TimeStamp = DateTime.Now,
                });
            }

            return Ok(loginResponse);

        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet(ApiEndPointConstant.Authentication.Info)]

        public async Task<IActionResult> GetUserInfo()
        {
            var currentUser = _httpContextAccessor.HttpContext.User;
            if (currentUser is null) return Unauthorized("Email Or Role Invalid!!!");

            var userId = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = currentUser.FindFirst(ClaimTypes.Email)?.Value;
            var role = currentUser.FindFirst(ClaimTypes.Role)?.Value;
            if (role is null || userId is null || email is null) return Unauthorized("Email Or Role Invalid!!!");
            if (role.Equals("customer"))
            {
                GetCustomerResponse customerResponse = await _customerService.GetCustomerByAccountId(userId);
                return Ok(customerResponse);
            }
            if (role.Equals("host"))
            {
                GetHostPartyResponse hostResponse = await _hostPartyService.GetHostPartyByAccountId(userId);
                return Ok(hostResponse);
            }
            if (role.Equals("admin"))
            {
                return Ok(new
                {
                    id = userId
                });
            }
            return Unauthorized("Email Or Role Invalid!!!");
        }
    }
