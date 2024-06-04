using Hairhub.Domain.Dtos.Requests.Authentication;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IAuthenticationService
    {
        public Task<LoginResponse> Login(string userName, string password);
        public Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest);
        public Task<bool> Logout(LogoutRequest logoutRequest);
        public Task<FetchUserResponse> FetchUser(string accessToken);
    }
}
