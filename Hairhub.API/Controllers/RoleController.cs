using Hairhub.API.Constants;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hairhub.API.Controllers
{
    [Route(ApiEndPointConstant.Role.RolesEndpoint + "/[action]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService roleService;

        public RoleController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await roleService.GetRoles();
            if (roles == null)
            {
                return BadRequest();
            }
            return Ok(roles);        
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var role = await roleService.GetRoleById(id);
            if (role == null)
            {
                return BadRequest();
            }
            return Ok(role);
        }
    }
}
