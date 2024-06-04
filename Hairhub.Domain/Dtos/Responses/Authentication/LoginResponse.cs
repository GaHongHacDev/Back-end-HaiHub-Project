using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Authentication
{
    public class LoginResponse
    {
        public string AccessToken {  get; set; } 
        public string RefreshToken {  get; set; }
        public Guid AccountId { get; set; }
    }
}
