using Hairhub.Domain.Entitities;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Hairhub.API.Controllers
{
    [Route("/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private IAccountService accountService;

        public AuthenticationController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        // POST api/user/login
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] Account user)
        {
            var token = accountService.Login(user.Username, user.Password);

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
    }
}
