using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Otps;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.AppointmentDetailVoucher;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Dtos.Responses.Otps;
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
            CreateMap<Customer, CustomerResponse>().ReverseMap();

            //Schedule
            CreateMap<Schedule, GetScheduleResponse>().ReverseMap();

            CreateMap<SalonEmployee, SalonEmployeeResponseS>().ReverseMap();

            //Feedback
            CreateMap<Feedback, GetFeedbackResponse>().ReverseMap();

            CreateMap<Customer, CustomerResponseF>().ReverseMap();

            CreateMap<AppointmentDetail, AppointmentDetailResponseF>().ReverseMap();

            //AppointmentDetailVoucher
            CreateMap<AppointmentDetailVoucher, GetAppointmentDetailVoucherResponse>().ReverseMap();

            CreateMap<Voucher, VoucherResponseA>().ReverseMap();

            CreateMap<AppointmentDetail, AppointmentResponseA>().ReverseMap();

            //Appointment
            CreateMap<GetAppointmentResponse, Appointment>().ReverseMap();
            CreateMap<CreateAppointmentRequest, Appointment>().ReverseMap();
            CreateMap<CreateAppointmentResponse, Appointment>().ReverseMap();
            CreateMap<UpdateAppointmentRequest, Appointment>().ReverseMap();
            CreateMap<AppointmentDetailRequest, AppointmentDetail>().ReverseMap();

            //OTP
            CreateMap<SendOtpEmailRequest, OTP>().ReverseMap();
            CreateMap<SendOtpEmailResponse, OTP>().ReverseMap();

            //AppointmentDetail
            CreateMap<ServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<AppointmentResponse, Appointment>().ReverseMap();
            CreateMap<SalonEmployeeResponse, SalonEmployee>().ReverseMap();
            CreateMap<GetAppointmentDetailResponse, AppointmentDetail>().ReverseMap();
            CreateMap<CreateAppointmentDetailRequest, AppointmentDetail>().ReverseMap();
            CreateMap<AppointmentDetail, CreateAppointmentDetailResponse>().ReverseMap();
            CreateMap<UpdateAppointmentDetailRequest, AppointmentDetail>().ReverseMap();
            CreateMap<GetAppointmentResponse, AppointmentDetail>().ReverseMap();
        }
    }
}
