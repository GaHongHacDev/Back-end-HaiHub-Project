using AutoMapper;
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
using Microsoft.EntityFrameworkCore;

namespace Hairhub.Service.Services.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IPaginate<GetCustomerResponse>> GetCustomers(int page, int size)
        {
            var customerEntities = await _unitOfWork.GetRepository<Customer>()
           .GetPagingListAsync(
               include: query => query.Include(s => s.Account),
               page: page,
               size: size
           );

            var paginateResponse = new Paginate<GetCustomerResponse>
            {
                Page = customerEntities.Page,
                Size = customerEntities.Size,
                Total = customerEntities.Total,
                TotalPages = customerEntities.TotalPages,
                Items = _mapper.Map<IList<GetCustomerResponse>>(customerEntities.Items)
            };

            return paginateResponse;
        }

        public async Task<GetCustomerResponse>? GetCustomerById(Guid id)
        {
            var customerEntity = await _unitOfWork
                .GetRepository<Customer>()
                .SingleOrDefaultAsync(
                    predicate: x => x.Id.Equals(id)
                 );
            if (customerEntity == null)
                return null;

            return _mapper.Map<GetCustomerResponse>(customerEntity);
        }
    }
}
