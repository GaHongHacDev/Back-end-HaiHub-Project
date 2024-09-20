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
using Hairhub.Domain.Exceptions;
using Hairhub.Domain.Dtos.Responses.Accounts;
using AutoMapper;
using Hairhub.Domain.Enums;
using Hairhub.Domain.Dtos.Responses.Appointments;
using CloudinaryDotNet.Core;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;

namespace Hairhub.Service.Services.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuaration;
        private readonly IMapper _mapper;

        public AuthenticationService(IUnitOfWork unitOfWork, IConfiguration configuaration, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _configuaration = configuaration;
            _mapper = mapper;
        }

        public async Task<FetchUserResponse> FetchUser(string accessToken)
        {
            var refreshTokenEntity = await _unitOfWork.GetRepository<RefreshTokenAccount>()
                .SingleOrDefaultAsync(
                                        predicate: x => x.AccessToken.Equals(accessToken) && x.Expires >= DateTime.Now,
                                        include: x => x.Include(y => y.Account.Role));
            if (refreshTokenEntity == null)
            {
                throw new NotFoundException("Không tìm thấy access token!");
            }
            var account = refreshTokenEntity.Account;
            SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
            Customer customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
            Admin admin = await _unitOfWork.GetRepository<Admin>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
            SalonEmployee salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);

            if (salonOwner == null && customer == null && admin == null && salonEmployee==null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }
            return new FetchUserResponse()
            {
                AccountId = account.Id,
                RoleName = account.Role?.RoleName,
                CustomerResponse = customer != null ? _mapper.Map<CustomerLoginResponse>(customer) : null,
                SalonOwnerResponse = salonOwner != null ? _mapper.Map<SalonOwnerLoginResponse>(salonOwner) : null,
                AdminResponse = admin != null ? _mapper.Map<AdminLoginResponse>(admin) : null,
                SalonEmployeeResponse = salonEmployee !=null ? _mapper.Map<SalonEmployeeResponse>(salonEmployee) : null,
            };
        }

        private string GenerateToken(string username, string roleName)
        {
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var key = Encoding.UTF8.GetBytes(_configuaration["JWTSettings:Key"]);

            //var tokenDescriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new ClaimsIdentity(new Claim[]
            //    {
            //        new Claim(ClaimTypes.Name, username),
            //        new Claim(ClaimTypes.Email, username),
            //        new Claim(ClaimTypes.Role, roleName)
            //    }),
            //    // Time Access Totken
            //    Expires = DateTime.UtcNow.AddMinutes(10),
            //    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            //};
            //var token = tokenHandler.CreateToken(tokenDescriptor);
            //return tokenHandler.WriteToken(token);


            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuaration["JWTSettings:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, roleName)
                }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                Issuer = _configuaration["JWTSettings:Issuer"], 
                Audience = _configuaration["JWTSettings:Audience"], 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<LoginResponse> Login(string userName, string password)
        {

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(
                predicate: u => u.UserName == userName && u.Password == password,
                include: x => x.Include(a => a.Role));
            // return null if user not found
            if (account == null)
            {
                return null;
            }
            SalonOwner salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
            Customer customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
            Admin admin = await _unitOfWork.GetRepository<Admin>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
            SalonEmployee salonEmployee = await _unitOfWork.GetRepository<SalonEmployee>().SingleOrDefaultAsync(predicate: x => x.AccountId == account.Id);
            if (salonOwner == null && customer == null && admin == null && salonEmployee==null)
            {
                throw new NotFoundException("Không tìm thấy tài khoản");
            }
            // authentication successful so generate jwt token and refresh token
            var accessToken = GenerateToken(account.UserName, account.Role.RoleName!);
            var refreshToken = GenerateRefreshToken();
            var newRefrehToken = new RefreshTokenAccount()
            {
                Id = Guid.NewGuid(),
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccountId = account.Id,
                Expires = DateTime.UtcNow.AddDays(30),
            };
            await _unitOfWork.GetRepository<RefreshTokenAccount>().InsertAsync(newRefrehToken);
            bool isInsert = await _unitOfWork.CommitAsync() > 0;
            if (!isInsert)
            {
                throw new Exception("Cannot insert token to DB");
            }

            return new LoginResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccountId = account.Id,
                RoleName = account.Role?.RoleName,
                CustomerResponse = customer != null ? _mapper.Map<CustomerLoginResponse>(customer) : null,
                SalonOwnerResponse = salonOwner != null ? _mapper.Map<SalonOwnerLoginResponse>(salonOwner) : null,
                AdminResponse = admin != null ? _mapper.Map<AdminLoginResponse>(admin) : null,
                SalonEmployeeResponse = salonEmployee != null ? _mapper.Map<SalonEmployeeResponse>(salonEmployee) : null,
            };
        }

        public async Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var refreshTokenEntity = await _unitOfWork.GetRepository<RefreshTokenAccount>().SingleOrDefaultAsync(
                                                predicate: x => x.RefreshToken == refreshTokenRequest.RefreshToken
                                                && x.Expires >= DateTime.Now);
            if (refreshTokenEntity == null)
            {
                throw new Exception("RefreshToken not found or expired");
            }

            var account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == refreshTokenEntity.AccountId, include: x=>x.Include(s=>s.Role));
            if (account == null)
            {
                throw new Exception("Account not found or expired");
            }

            var accessToken = GenerateToken(account.UserName, account.Role.RoleName!);
            refreshTokenEntity.AccessToken = accessToken;
            _unitOfWork.GetRepository<RefreshTokenAccount>().UpdateAsync(refreshTokenEntity);
            bool isUpdate = await _unitOfWork.CommitAsync() > 0;
            if (!isUpdate)
            {
                throw new Exception("Cannot insert new access token to DB");
            }
            return new RefreshTokenResponse() { AccessToken = refreshTokenEntity.AccessToken, RefreshToken = refreshTokenEntity.RefreshToken };
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

        public async Task<bool> Logout(LogoutRequest logoutRequest)
        {
            var refreshTokens = await _unitOfWork.GetRepository<RefreshTokenAccount>().GetListAsync(predicate: x => x.RefreshToken.Equals(logoutRequest.RefreshToken));
            if (refreshTokens == null)
            {
                throw new NotFoundException("Refresh token not found!");
            }
            _unitOfWork.GetRepository<RefreshTokenAccount>().DeleteRangeAsync(refreshTokens);
            bool isDelete = await _unitOfWork.CommitAsync() > 0;
            return isDelete;
        }
    }
}
