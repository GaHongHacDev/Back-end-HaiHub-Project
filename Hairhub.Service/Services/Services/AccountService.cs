using Hairhub.Domain.Entitities;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration configuaration;
        private readonly IUnitOfWork unitOfWork;

        public AccountService(IConfiguration configuaration, IUnitOfWork unitOfWork)
        {
            this.configuaration = configuaration;
            this.unitOfWork = unitOfWork;
        }

        public async Task<string> Login(string userName, string password)
        {

            var user = await unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: u => u.Username == userName && u.Password == password);
    
            // return null if user not found
            if (user == null)
            {
                return string.Empty;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuaration["JWTSettings:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.RoleName)
                }),

                Expires = DateTime.UtcNow.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            return user.Token;
            
        }

        public async Task<(Customer, Account)> RegisterAccountCustomer(Customer customer, Account account)
        {
            var role = await unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate:x=>x.RoleName.Equals("Customer"));
            if (role == null)
            {
                throw new Exception("Role Name is not exist!");
            }
            account.Id = Guid.NewGuid();
            account.RoleId = role.RoleId;
            account.IsActive = true;
            customer.Id = Guid.NewGuid();
            customer.AccountId = account.Id;
            await unitOfWork.GetRepository<Customer>().InsertAsync(customer);
            await unitOfWork.GetRepository<Account>().InsertAsync(account);
            return (customer, account);
        }

        public async Task<(SalonOwner, Account)> RegisterAccountSalonOwner(SalonOwner salonOwner, Account account)
        {
            var role = await unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleName.Equals("SalonOwner"));
            account.Id = Guid.NewGuid();
            account.RoleId = role.RoleId;
            account.IsActive = true;
            salonOwner.Id = Guid.NewGuid();
            salonOwner.AccountId = account.Id;
            await unitOfWork.GetRepository<SalonOwner>().InsertAsync(salonOwner);
            await unitOfWork.GetRepository<Account>().InsertAsync(account);
            return (salonOwner, account);
        }
    }
}
