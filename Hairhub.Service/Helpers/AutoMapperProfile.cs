
﻿using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Otps;
using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Dtos.Requests.SalonOwners;
using Hairhub.Domain.Dtos.Requests.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Dtos.Responses.AppointmentDetails;
using Hairhub.Domain.Dtos.Responses.Appointments;
using Hairhub.Domain.Dtos.Responses.AppointmentDetailVoucher;
using Hairhub.Domain.Dtos.Responses.Customers;
using Hairhub.Domain.Dtos.Responses.Otps;
using Hairhub.Domain.Dtos.Responses.SalonEmployees;
using Hairhub.Domain.Dtos.Responses.SalonInformations;
using Hairhub.Domain.Dtos.Responses.SalonOwners;
using Hairhub.Domain.Dtos.Responses.ServiceHairs;
using Hairhub.Domain.Dtos.Responses.Feedbacks;
using Hairhub.Domain.Dtos.Responses.Schedules;
using Hairhub.Domain.Entitities;
using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Requests.Voucher;
using Hairhub.Domain.Dtos.Responses.Config;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Dtos.Requests.Schedule;
using Hairhub.Domain.Dtos.Responses.Payment;
using Hairhub.Domain.Dtos.Requests.Payment;
using SalonOwner = Hairhub.Domain.Entitities.SalonOwner;
using Config = Hairhub.Domain.Entitities.Config;
using Hairhub.Domain.Dtos.Responses.Authentication;

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
            CreateMap<GetAccountResponse, Account>().ReverseMap();
            CreateMap<GetAccountResponse, Customer>().ReverseMap();
            CreateMap<GetAccountResponse, SalonOwner>().ReverseMap();
            CreateMap<FetchUserResponse, Customer>().ReverseMap();
            CreateMap<FetchUserResponse, SalonOwner>().ReverseMap();
            CreateMap<FetchUserResponse, Account>().ReverseMap(); 
            CreateMap<UpdateAccountResponse, SalonOwner>().ReverseMap(); 
            CreateMap<UpdateAccountResponse, Customer>().ReverseMap();
            CreateMap<CustomerLoginResponse, Customer>().ReverseMap();
            CreateMap<SalonOwnerLoginResponse, SalonOwner>().ReverseMap();

            //Customer
            CreateMap<GetCustomerResponse, Customer>().ReverseMap();
            CreateMap<Customer, CustomerResponse>().ReverseMap();

            //Schedule
            CreateMap<Schedule, GetScheduleResponse>().ReverseMap(); 
            CreateMap<SalonEmployee, SalonEmployeeResponseS>().ReverseMap();
            CreateMap<SalonInformationSchedule, SalonInformation>().ReverseMap();

            //Feedback
            CreateMap<Feedback, GetFeedbackResponse>().ReverseMap();
            CreateMap<Customer, CustomerResponseF>().ReverseMap();
            CreateMap<AppointmentDetail, AppointmentDetailResponseF>().ReverseMap();

            //AppointmentDetailVoucher
            CreateMap<AppointmentDetailVoucher, GetAppointmentDetailVoucherResponse>().ReverseMap();
            CreateMap<Voucher, VoucherResponseA>().ReverseMap();
            CreateMap<Appointment, AppointmentResponseA>().ReverseMap();

            //Appointment
            CreateMap<GetAppointmentResponse, Appointment>().ReverseMap();
            CreateMap<CreateAppointmentRequest, Appointment>().ReverseMap();
            CreateMap<CreateAppointmentResponse, Appointment>().ReverseMap();
            CreateMap<UpdateAppointmentRequest, Appointment>().ReverseMap(); 
            CreateMap<AppointmentDetailRequest, AppointmentDetail>().ReverseMap();
            CreateMap<GetAppointmentByAccountIdResponse, Appointment>().ReverseMap();

            //SalonOwner
            CreateMap<GetSalonOwnerResponse, SalonOwner>().ReverseMap();
            CreateMap<CreateSalonOwnerRequest, SalonOwner>().ReverseMap();
            CreateMap<CreateSalonOwnerResponse, SalonOwner>().ReverseMap();
            CreateMap<UpdateSalonOwnerRequest, SalonOwner>().ReverseMap();
            CreateMap<Account, AccountSalonOwnerResponse> ().ReverseMap();

            //SalonEmployee
            CreateMap<SalonEmployee, GetSalonEmployeeResponse>()
                        .ForMember(dest => dest.ServiceHairs, opt => opt.MapFrom(src => src.ServiceEmployees.Select(se => se.ServiceHair)));
            CreateMap<CreateSalonEmployeeRequest, SalonEmployee>().ReverseMap();
            CreateMap<UpdateSalonEmployeeRequest, SalonEmployee>().ReverseMap();
            CreateMap<EmployeeRequest, SalonEmployee>().ReverseMap();
            CreateMap<ScheduleEmployee, Schedule>().ReverseMap();
            CreateMap<ScheduleEmployeeResponse, Schedule>().ReverseMap();


            //SalonInformation
            CreateMap<GetSalonInformationResponse, SalonInformation>().ReverseMap();
            CreateMap<CreateSalonInformationRequest, SalonInformation>().ReverseMap();
            CreateMap<CreateSalonInformationResponse, SalonInformation>().ReverseMap();
            CreateMap<UpdateSalonInformationRequest, SalonInformation>().ReverseMap();
            CreateMap<SalonOwnerSalonInformationResponse, SalonOwner>().ReverseMap();
            CreateMap<CreateScheduleRequest, Schedule>().ReverseMap(); 
            CreateMap<SalonInformationSchedule, SalonInformation>().ReverseMap();
            CreateMap<SalonInformation, SearchSalonByNameAddressServiceResponse>()
                       .ForMember(dest => dest.Services, opt => opt.Ignore())
                       .ForMember(dest => dest.Vouchers, opt => opt.Ignore());
            CreateMap<SearchSalonServiceResponse, ServiceHair>().ReverseMap();
            CreateMap<SearchSalonVoucherRespnse, Voucher>().ReverseMap();

            //ServiceHair
            CreateMap<GetServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<CreateServiceHairRequest, ServiceHair>().ReverseMap();
            CreateMap<CreateServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<UpdateServiceHairRequest, ServiceHair>().ReverseMap();

            //OTP
            CreateMap<SendOtpEmailRequest, OTP>().ReverseMap();
            CreateMap<SendOtpEmailResponse, OTP>().ReverseMap();

            //AppointmentDetail
            CreateMap<ServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<SalonEmployeeResponse, SalonEmployee>().ReverseMap();
            CreateMap<GetAppointmentDetailResponse, AppointmentDetail>().ReverseMap();
            CreateMap<CreateAppointmentDetailRequest, AppointmentDetail>().ReverseMap();
            CreateMap<AppointmentDetail, CreateAppointmentDetailResponse>().ReverseMap();
            CreateMap<UpdateAppointmentDetailRequest, AppointmentDetail>().ReverseMap();
            CreateMap<GetAppointmentResponse, AppointmentDetail>().ReverseMap();

            //Voucher
            CreateMap<CreateVoucherRequest, Voucher>().ReverseMap();
            CreateMap<Voucher, CreateVoucherResponse>().ReverseMap();
            CreateMap<UpdateVoucherRequest, Voucher>().ReverseMap();
            CreateMap<Voucher, UpdateVoucherResponse>().ReverseMap();
            CreateMap<GetVoucherResponse, Voucher>().ReverseMap();
            CreateMap<Voucher, GetVoucherResponse>().ReverseMap();

            //Config
            CreateMap<CreateConfigRequest, Config>().ReverseMap();
            CreateMap<Config, CreateConfigResponse>().ReverseMap();
            CreateMap<UpdateConfigRequest, Config>().ReverseMap();
            CreateMap<Config, UpdateConfigResponse>().ReverseMap();


            //Payment
            CreateMap<Payment, CreatePaymentRequest>().ReverseMap();
            CreateMap<ResponsePayment, Payment>().ReverseMap();
            CreateMap<Payment, SavePaymentInfor>().ReverseMap();
            CreateMap<SavePaymentInfor, Payment>().ReverseMap();

        }
    }
}

