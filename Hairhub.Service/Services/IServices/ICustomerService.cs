﻿using Hairhub.Domain.Dtos.Requests.Customers;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Dtos.Responses.Dashboard;
using Hairhub.Domain.Entitities;
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

        //Task<DataOfMonths> GetCustomerofYearByAdmin(int year);

        Task<bool> SaveAsCustomerImageHistory(CustomerImageHistoryRequest request);


        Task<IPaginate<CustomerImageHistoryResponse>> GetCustomerImagesHistory(Guid customerId, int page, int size);
    }
}
