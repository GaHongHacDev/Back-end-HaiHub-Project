using Hairhub.Domain.Entitities;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork _unitOfWork)
        {
            this._unitOfWork = _unitOfWork;
        }      

        public async Task<ICollection<Role>> GetRoles()
        {
            var roles = await _unitOfWork.GetRepository<Role>().GetListAsync();
            return roles;
        }

        public async Task<Role> GetRoleById(Guid id)
        {            
            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleId.Equals(id));
            return role;
        }
    }
}
