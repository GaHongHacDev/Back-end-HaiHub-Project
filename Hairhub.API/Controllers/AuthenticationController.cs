using AutoMapper;
using Hairhub.API.Constants;
using Hairhub.API.Dtos.Requests.Accounts;
using Hairhub.API.Dtos.Responses.Accounts;
using Hairhub.Domain.Entitities;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static Hairhub.API.Constants.ApiEndPointConstant;


namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Account.AccountsEndpoint + "/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private IAccountService _accountService;

        public AuthenticationController(IMapper _mapper, IAccountService accountService) : base(_mapper)
        {
            _accountService = accountService;
        }



        // POST api/user/login
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] Domain.Entitities.Account user)
        {
            var token = _accountService.Login(user.Username, user.Password);

            if (token == null || String.IsNullOrWhiteSpace(token.ToString()))
                return BadRequest(new { message = "User name or password is incorrect" });

            return Ok(token);
        }

        [HttpGet]
        [Authorize(Roles = "admin,manager")]
        public ActionResult<IEnumerable<string>> Load()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAccount([FromBody] CreateAccountRequest createAccountRequest)
        {
            try
            {
                if (string.IsNullOrEmpty(createAccountRequest.RoleName))
                {
                    var accountEntity = mapper.Map<Domain.Entitities.Account>(createAccountRequest);
                    if ("Customer".Equals(createAccountRequest))
                    {
                        var customerEntitiy = _mapper.Map<Domain.Entitities.Customer>(createAccountRequest);
                        (customerEntitiy, accountEntity) = await _accountService.RegisterAccountCustomer(customerEntitiy, accountEntity);
                        var response = _mapper.Map(customerEntitiy, accountEntity);
                        return Ok(response);
                    }
                    else if ("SalonOwner".Equals(createAccountRequest))
                    {
                        var salonOwnerEntity = _mapper.Map<SalonOwner>(createAccountRequest);
                        (salonOwnerEntity, accountEntity) = await _accountService.RegisterAccountSalonOwner(salonOwnerEntity, accountEntity);
                        var response = _mapper.Map(salonOwnerEntity, accountEntity);
                        return Ok(response);
                    }
                }
                return BadRequest("Can not create account!");
            }catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
