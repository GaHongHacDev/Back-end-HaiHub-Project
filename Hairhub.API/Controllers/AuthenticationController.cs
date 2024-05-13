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
        private IUserService _userService;

        public AuthenticationController(IUserService userService)
        {
            _userService = userService;
        }

        // POST api/user/login
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] Account user)
        {
            var token = _userService.Login(user.Username, user.Password);

            if (token == null || token == String.Empty)
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
