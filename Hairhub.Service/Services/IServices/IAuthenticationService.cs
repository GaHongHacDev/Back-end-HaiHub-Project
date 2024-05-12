using Hairhub.Domain.Entitities;
using Hairhub.Domain.Payload.Request;
using Hairhub.Domain.Payload.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IAuthenticationService
    {
        Task<LoginReponse> Login(LoginRequest loginRequest);
        Task<Account> CreateAccount(string email, string role);
    }
}
