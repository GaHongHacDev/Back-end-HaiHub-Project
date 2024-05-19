using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Entitities;

namespace Hairhub.Service.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Account
            CreateMap<CreateAccountRequest, Customer>().ReverseMap();

            CreateMap<CreateAccountRequest, SalonOwner>().ReverseMap();

            CreateMap<CreateAccountRequest, Account>().ReverseMap();

            CreateMap<CreateAccountRequest, CreateAccountResponse>().ReverseMap();
            
            //Customer
            CreateMap<GetCustomerResponse, Customer>().ReverseMap();


        }
    }
}
