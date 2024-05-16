using Hairhub.Domain.Entitities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface IAccountService
    {
        Task<string> Login(string userName, string password);
        Task<(Customer, Account)> RegisterAccountCustomer(Customer customer, Account account);
        Task<(SalonOwner, Account)> RegisterAccountSalonOwner(SalonOwner salonOwner, Account account);
    }
}
