using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Requests.Accounts
{
    public class CheckLoginGoogleRequest
    {
        public string IdToken { get; set; }
        public string? type { get; set; }
    }
}
