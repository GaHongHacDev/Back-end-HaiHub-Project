using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hairhub.Service.Services.IServices
{
    public interface ICustomerService
    {
        Task<IPaginate<GetCustomerResponse>> GetCustomers(int page, int size);
        Task<GetCustomerResponse>? GetCustomerById(Guid id);
        Task<bool> CheckInByCustomer(string dataAES, Guid customerId);
    }
}
