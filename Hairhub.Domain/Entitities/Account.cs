using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public partial class Account
    {
        public string? Id { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public string? Role { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool? IsDeleted { get; set; }
    }
}
