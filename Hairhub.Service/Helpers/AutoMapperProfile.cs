using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.AppointmentDetailVoucher;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Schedules;
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

            //Schedule
            CreateMap<Schedule, GetScheduleResponse>().ReverseMap();

            CreateMap<SalonEmployee, SalonEmployeeResponse>().ReverseMap();

            //Feedback
            CreateMap<Feedback, GetFeedbackResponse>().ReverseMap();

            CreateMap<Customer, CustomerResponse>().ReverseMap();

            CreateMap<AppointmentDetail, AppointmentDetailResponseF>().ReverseMap();

            //AppointmentDetailVoucher
            CreateMap<AppointmentDetailVoucher, GetAppointmentDetailVoucherResponse>().ReverseMap();

            CreateMap<Voucher, VoucherResponse>().ReverseMap();

            CreateMap<AppointmentDetail, AppointmentDetailResponseA>().ReverseMap();


        }
    }
}
