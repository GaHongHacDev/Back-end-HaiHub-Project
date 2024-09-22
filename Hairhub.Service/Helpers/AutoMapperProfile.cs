
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
using Hairhub.Domain.Dtos.Requests.Approval;
using Hairhub.Domain.Dtos.Responses.Approval;
using Hairhub.Domain.Dtos.Responses.Reports;
using Hairhub.Domain.Dtos.Requests.Reports;

namespace Hairhub.Service.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //Admin 
            CreateMap<AdminLoginResponse, Admin>().ReverseMap();

            //Account
            CreateMap<CreateAccountRequest, Customer>().ReverseMap();
            CreateMap<CreateAccountRequest, SalonOwner>().ReverseMap();
            CreateMap<CreateAccountRequest, Account>().ReverseMap();
            CreateMap<CreateAccountRequest, CreateAccountResponse>().ReverseMap();
            CreateMap<GetAccountResponse, Account>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<GetAccountResponse, Customer>().ReverseMap();
            CreateMap<GetAccountResponse, SalonOwner>().ReverseMap();
            CreateMap<GetAccountResponse, SalonEmployee>().ReverseMap();
            CreateMap<FetchUserResponse, Customer>().ReverseMap();
            CreateMap<FetchUserResponse, SalonOwner>().ReverseMap();
            CreateMap<FetchUserResponse, Account>().ReverseMap(); 
            CreateMap<CustomerLoginResponse, Customer>().ReverseMap();
            CreateMap<SalonOwnerLoginResponse, SalonOwner>().ReverseMap();

            //Config
            CreateMap<GetConfigResponse, Config>().ReverseMap();

            //Customer
            CreateMap<GetCustomerResponse, Customer>().ReverseMap();
            CreateMap<CustomerAppointment, Customer>().ReverseMap();

            //Schedule
            CreateMap<Schedule, GetScheduleResponse>().ReverseMap(); 

            //Appointment
            CreateMap<CreateAppointmentRequest, Appointment>().ReverseMap();
            CreateMap<CreateAppointmentResponse, Appointment>().ReverseMap();
            CreateMap<UpdateAppointmentRequest, Appointment>().ReverseMap();
            CreateMap<Appointment, GetAppointmentResponse>()
                .ForMember(dest => dest.SalonInformation, opt => opt.MapFrom(src => src.AppointmentDetails.FirstOrDefault()!.SalonEmployee.SalonInformation));
            CreateMap<AppointmentTransaction, Appointment>().ReverseMap();

            //SalonOwner
            CreateMap<GetSalonOwnerResponse, SalonOwner>().ReverseMap();
            CreateMap<CreateSalonOwnerRequest, SalonOwner>().ReverseMap();
            CreateMap<CreateSalonOwnerResponse, SalonOwner>().ReverseMap();
            CreateMap<UpdateSalonOwnerRequest, SalonOwner>().ReverseMap();
            CreateMap<Account, AccountResponse> ().ReverseMap();

            //SalonEmployee
            CreateMap<SalonEmployee, GetSalonEmployeeResponse>()
                        .ForMember(dest => dest.ServiceHairs, opt => opt.MapFrom(src => src.ServiceEmployees.Select(se => se.ServiceHair)));
            CreateMap<CreateSalonEmployeeRequest, SalonEmployee>().ReverseMap();
            CreateMap<UpdateSalonEmployeeRequest, SalonEmployee>().ReverseMap();
            CreateMap<EmployeeRequest, SalonEmployee>().ReverseMap();
            CreateMap<ScheduleEmployee, Schedule>().ReverseMap();
            CreateMap<ScheduleEmployeeResponse, Schedule>().ReverseMap();
            CreateMap<SalonEmployee, GetEmployeeHighRatingResponse>().ReverseMap();


            //SalonInformation
            CreateMap<SalonInformation, GetSalonInformationResponse>()
                .ForMember(dest => dest.schedules, opt => opt.MapFrom(src => src.Schedules));
            CreateMap<CreateSalonInformationRequest, SalonInformation>().ReverseMap();
            CreateMap<CreateSalonInformationResponse, SalonInformation>().ReverseMap();
            CreateMap<UpdateSalonInformationRequest, SalonInformation>().ReverseMap();
            CreateMap<SalonOwnerSalonInformationResponse, SalonOwner>().ReverseMap();
            CreateMap<CreateScheduleRequest, Schedule>().ReverseMap();
            CreateMap<SalonInformation, SearchSalonByNameAddressServiceResponse>()
                       .ForMember(dest => dest.Services, opt => opt.Ignore())
                       .ForMember(dest => dest.Vouchers, opt => opt.Ignore());
            CreateMap<SearchSalonServiceResponse, ServiceHair>().ReverseMap();
            CreateMap<SearchSalonVoucherRespnse, Voucher>().ReverseMap();
            CreateMap<AppointmentSalon, SalonInformation>().ReverseMap(); 
            CreateMap<SalonInformation, GetSalonInformationResponse>().ReverseMap();
            CreateMap<SalonInformation, SalonSuggesstionResponse>().ReverseMap();
            CreateMap<SalonEmployee, ReviewRevenueEmployee>();

            //ServiceHair
            CreateMap<GetServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<CreateServiceHairRequest, ServiceHair>().ReverseMap();
            CreateMap<CreateServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<UpdateServiceHairRequest, ServiceHair>().ReverseMap();
            CreateMap<ServiceHairAvalibale, ServiceHair>().ReverseMap();

            //OTP
            CreateMap<SendOtpEmailRequest, OTP>().ReverseMap();
            CreateMap<SendOtpEmailResponse, OTP>().ReverseMap();

            //AppointmentDetail
            CreateMap<ServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<SalonEmployeeResponse, SalonEmployee>().ReverseMap();
            CreateMap<GetAppointmentDetailResponse, AppointmentDetail>().ReverseMap();
            CreateMap<CreateAppointmentDetailRequest, AppointmentDetail>().ReverseMap();
           // CreateMap<AppointmentDetail, CreateAppointmentDetailResponse>().ReverseMap();
            CreateMap<UpdateAppointmentDetailRequest, AppointmentDetail>().ReverseMap();
            CreateMap<GetAppointmentResponse, AppointmentDetail>().ReverseMap();

            //Voucher
            CreateMap<CreateVoucherRequest, Voucher>().ReverseMap();
            CreateMap<Voucher, CreateVoucherResponse>().ReverseMap();
            CreateMap<UpdateVoucherRequest, Voucher>().ReverseMap();
            CreateMap<GetVoucherResponse, Voucher>().ReverseMap();
            CreateMap<Voucher, GetVoucherResponse>().ReverseMap();

            //Config
            CreateMap<CreateConfigRequest, Config>().ReverseMap();
            CreateMap<Config, CreateConfigResponse>().ReverseMap();
            CreateMap<UpdateConfigRequest, Config>().ReverseMap();


            //Payment
            CreateMap<Payment, CreatePaymentRequest>().ReverseMap();
            CreateMap<Payment, ResponsePayment>()
                .ForMember(dest => dest.SalonOwners, opt => opt.MapFrom(src => src.SalonOwner)) // Ánh xạ cho SalonOwner
                .ForMember(dest => dest.Config, opt => opt.MapFrom(src => src.Config)) // Ánh xạ cho Config
                .ForMember(dest => dest.SalonInformation, opt => opt.MapFrom(src => src.SalonOwner.SalonInformations.FirstOrDefault())) // Ánh xạ cho SalonInformation
                .ReverseMap();
            CreateMap<Payment, SavePaymentInfor>().ReverseMap();
            CreateMap<SavePaymentInfor, Payment>().ReverseMap();
            CreateMap<SalonOwnerPaymentResponse, SalonOwner>().ReverseMap();
            CreateMap<Config, ConfigPaymentResponse>().ReverseMap();
            CreateMap<SalonInformation, SalonPaymentResponse>().ReverseMap();

            //Approval
            CreateMap<CreateApprovalRequest, Approval>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdateApprovalRequest, Approval>()
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate ?? DateTime.UtcNow));

            CreateMap<Approval, GetApprovalResponse>();

            //Report CreateReportRequest
            CreateMap<Report, GetReportResponse>()
                 .ForMember(dest => dest.FileReports, opt => opt.MapFrom(src => src.StaticFiles.ToList()));

            CreateMap<Report, UpdateAccountRequest>().ReverseMap(); 
            CreateMap<Report, CreateReportRequest>().ReverseMap();

            //Feedback
            CreateMap<Feedback, GetFeedbackResponse>()
                .ForMember(dest => dest.FileFeedbacks, opt => opt.MapFrom(src => src.StaticFiles.ToList()));

            //Static File
            CreateMap<StaticFile, FileReportResponse>().ReverseMap();
            CreateMap<StaticFile, FileFeedbackResponse>().ReverseMap();


            //Customer Image History
            CreateMap<StyleHairCustomer, CustomerImageHistoryResponse>()
            .ForMember(dest => dest.ImageStyles, opt => opt.MapFrom(src => src.ImageStyles))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));

            CreateMap<Customer, Customers>(); // Assuming Customer -> Customers mapping
            CreateMap<ImageStyle, ImageStyles>();
        }
    }
}

