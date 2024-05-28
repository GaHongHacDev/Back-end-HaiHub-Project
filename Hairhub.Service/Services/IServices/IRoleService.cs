using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IRoleService
    {
        Task<ICollection<Role>> GetRoles();
        Task<Role> GetRoleById(Guid id);
    }
}
