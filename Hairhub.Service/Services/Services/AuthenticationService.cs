using Hairhub.Domain.Entitities;
using Hairhub.Service.Repositories.IRepositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Hairhub.Service.Services.IServices;
using System.Security.Cryptography;
using Hairhub.Domain.Dtos.Requests.Authentication;
using Hairhub.Domain.Dtos.Responses.Authentication;

namespace Hairhub.Service.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuaration;

        public AuthenticationService(IUnitOfWork unitOfWork, IConfiguration configuaration)
        {
            _unitOfWork = unitOfWork;
            _configuaration = configuaration;
        }
        public async Task<LoginResponse> Login(string userName, string password)
        {

            var user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: u => u.Username == userName && u.Password == password,
                include: x => x.Include(a => a.Role));
            // return null if user not found
            if (user == null)
            {
                return null;
            }
            // authentication successful so generate jwt token and refresh token
            var accessToken = GenerateToken(user.Username, user.RoleId.ToString());
            var refreshToken = GenerateRefreshToken();
            await _unitOfWork.GetRepository<RefreshTokenAccount>().InsertAsync(new RefreshTokenAccount()
            {
                Id = new Guid(accessToken),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccountId = user.Id,
                Expires = DateTime.UtcNow.AddDays(30)
            });
            bool isInsert = await _unitOfWork.CommitAsync()>0;
            if (isInsert)
            {
                throw new Exception("Cannot insert token to DB");
            }
            return new LoginResponse() { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshTokenEntity =await _unitOfWork.GetRepository<RefreshTokenAccount>().SingleOrDefaultAsync(
                                                predicate: x=>x.RefreshToken == refreshTokenRequest.RefreshToken
                                                && x.IsActive == true);
            if (refreshTokenEntity == null)
            {
                throw new Exception("RefreshToken not found or expired");
            }

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x=>x.Id == refreshTokenEntity.AccountId);
            if (account == null)
            {
                throw new Exception("Account not found or expired");
            }

            var accessToken = GenerateToken(account.Username, account.RoleId.ToString());
            refreshTokenEntity.AccessToken = accessToken;
            await _unitOfWork.GetRepository<RefreshTokenAccount>().InsertAsync(refreshTokenEntity);
            bool isUpdate = await _unitOfWork.CommitAsync()>0;
            if (!isUpdate)
            {
                throw new Exception("Cannot insert new access token to DB");
            }
            return new RefreshTokenResponse() { AccessToken = accessToken };
        }

        private String GenerateToken(String username, String roleName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuaration["JWTSettings:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, roleName)
                }),

                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public async Task<string> Logout(string token)
        {
            return "";
        }
    }
}
