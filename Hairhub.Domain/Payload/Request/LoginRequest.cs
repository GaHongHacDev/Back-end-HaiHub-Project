using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Payload.Request
{
    public class LoginRequest
    {
        public string AccessToken { get; set; }
        public string TokenId { get; set; }
    }
}
