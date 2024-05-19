using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Otps;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Dtos.Responses.Otps;
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
            CreateMap<Customer, CustomerResponse>().ReverseMap();

            //Appointment
            CreateMap<GetAppointmentResponse, Appointment>().ReverseMap();
            CreateMap<CreateAppointmentRequest, Appointment>().ReverseMap();
            CreateMap<CreateAppointmentResponse, Appointment>().ReverseMap();
            CreateMap<UpdateAppointmentRequest, Appointment>().ReverseMap();

            //OTP
            CreateMap<SendOtpEmailRequest, OTP>().ReverseMap();
            CreateMap<SendOtpEmailResponse, OTP>().ReverseMap();


        }
    }
}
