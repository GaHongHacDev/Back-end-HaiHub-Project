using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Entitities
{
    public class RefreshTokenAccount
    {
        public Guid Id { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Expires { get; set; }
        public bool IsActive
        {
            get { return DateTime.UtcNow <= Expires; }
            private set {}
        }
        public Guid AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}
