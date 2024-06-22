using Hairhub.Domain.Dtos.Responses.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Domain.Dtos.Responses.Accounts
{
    public class FetchUserResponse
    {
        public Guid AccountId { get; set; }
        public string RoleName { get; set; }
        public CustomerLoginResponse? CustomerResponse { get; set; }
        public SalonOwnerLoginResponse? SalonOwnerResponse { get; set; }
        public AdminLoginResponse? AdminResponse { get; set; }
    }
}
