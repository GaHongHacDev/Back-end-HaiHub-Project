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
using Microsoft.AspNetCore.Http;
using Hairhub.Domain.Dtos.Requests.Accounts;

namespace Hairhub.Service.Services.Services
{
    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuaration;
        private readonly IUnitOfWork _unitOfWork;

        public AccountService(IConfiguration configuaration, IUnitOfWork unitOfWork)
        {
            this._configuaration = configuaration;
            this._unitOfWork = unitOfWork;
        }

        public async Task<string> Login(string userName, string password)
        {

            var user = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: u => u.Username == userName && u.Password == password);
    
            // return null if user not found
            if (user == null)
            {
                return string.Empty;
            }

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuaration["JWTSettings:Key"]);

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
            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate:x=>x.RoleName.Equals("Customer"));
            if (role == null)
            {
                throw new Exception("Role Name is not exist!");
            }
            account.Id = Guid.NewGuid();
            account.RoleId = role.RoleId;
            account.IsActive = true;
            customer.Id = Guid.NewGuid();
            customer.AccountId = account.Id;
            await _unitOfWork.GetRepository<Customer>().InsertAsync(customer);
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();
            return (customer, account);
        }

        public async Task<(SalonOwner, Account)> RegisterAccountSalonOwner(SalonOwner salonOwner, Account account)
        {
            var role = await _unitOfWork.GetRepository<Role>().SingleOrDefaultAsync(predicate: x => x.RoleName.Equals("SalonOwner"));
            account.Id = Guid.NewGuid();
            account.RoleId = role.RoleId;
            account.IsActive = true;
            salonOwner.Id = Guid.NewGuid();
            salonOwner.AccountId = account.Id;
            await _unitOfWork.GetRepository<SalonOwner>().InsertAsync(salonOwner);
            await _unitOfWork.GetRepository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();
            return (salonOwner, account);
        }

        public async Task<bool> UpdateAccountById(Guid id, UpdateAccountRequest updateAccountRequest)
        {
            var customer = await _unitOfWork.GetRepository<Customer>().SingleOrDefaultAsync(predicate: x => x.Id == id);
            SalonOwner salonOwner;
            Guid accountId;
            if (customer == null)
            {
                salonOwner = await _unitOfWork.GetRepository<SalonOwner>().SingleOrDefaultAsync(predicate: x => x.Id == id);
                if(salonOwner == null)
                {
                    return false;
                }
                accountId = salonOwner.AccountId;
                salonOwner.FullName = updateAccountRequest.FullName;
                customer.Email = updateAccountRequest.Email;
                customer.Phone = updateAccountRequest.Phone;
                customer.Address = updateAccountRequest.Address;
                customer.HumanId = updateAccountRequest.HumanId;
                customer.Img = updateAccountRequest.Img;
                customer.BankAccount = updateAccountRequest.BankAccount;
                customer.BankName = updateAccountRequest.BankName;
            }
            else
            {
                accountId = customer.AccountId;
                customer.FullName = updateAccountRequest.FullName;
                customer.Email = updateAccountRequest.Email;
                customer.Phone = updateAccountRequest.Phone;
                customer.Address = updateAccountRequest.Address;
                customer.HumanId = updateAccountRequest.HumanId;
                customer.Img = updateAccountRequest.Img;
                customer.BankAccount = updateAccountRequest.BankAccount;
                customer.BankName = updateAccountRequest.BankName;
            }
            Account account = await _unitOfWork.GetRepository<Account>().SingleOrDefaultAsync(predicate: x => x.Id == accountId);

            if(account==null)
                throw new Exception("Discount was not found");
            //Update Account
            account.Username = updateAccountRequest.Username;
            account.Password = updateAccountRequest.Password;
            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;
            return isSuccessful;
        }
    }
}
