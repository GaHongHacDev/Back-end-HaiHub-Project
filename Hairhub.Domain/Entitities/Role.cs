using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class Role
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
    }
}
