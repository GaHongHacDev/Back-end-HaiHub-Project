using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Payload.Response
{
    public class LoginReponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public Account Account { get; set; }
    }
}
