using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Specifications;
using Hairhub.Service.Repositories.IRepositories;
using Hairhub.Service.Services.IServices;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork _unitOfWork)
        {
            _unitOfWork = _unitOfWork;
        }
        public async Task<IPaginate<GetCustomerResponse>> GetCustomers(int page, int size)
        {
            IPaginate<GetCustomerResponse> customersResponse = await _unitOfWork.GetRepository<Customer>()
            .GetPagingListAsync(
              selector: x => new GetCustomerResponse(x.Id, x.AccountId,x.FullName, x.DayOfBirth, x.Gender,
                                                     x.Email, x.Phone, x.Address, x.HumanId, x.Img, x.BankAccount, x.BankName),
              page: page,
              size: size,
              orderBy: x => x.OrderBy(x => x.FullName));
            return customersResponse;
        }

        public async Task<GetCustomerResponse> GetCustomerById(Guid id)
        {
            GetCustomerResponse CustomerResponse = await _unitOfWork
                .GetRepository<Customer>()
                .SingleOrDefaultAsync(
                    selector: x => new GetCustomerResponse(x.Id, x.AccountId, x.FullName, x.DayOfBirth, x.Gender,
                                                     x.Email, x.Phone, x.Address, x.HumanId, x.Img, x.BankAccount, x.BankName),
                    predicate: x => x.Id.Equals(id)
                 );
            if (CustomerResponse == null) throw new Exception("Customer is not found");
            return CustomerResponse;
        }
    }
}
