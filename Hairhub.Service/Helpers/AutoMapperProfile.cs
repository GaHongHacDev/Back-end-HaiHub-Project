
﻿using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Requests.AppointmentDetails;
using Hairhub.Domain.Dtos.Requests.Appointments;
using Hairhub.Domain.Dtos.Requests.Otps;
using Hairhub.Domain.Dtos.Requests.SalonEmployees;
using Hairhub.Domain.Dtos.Requests.SalonInformations;
using Hairhub.Domain.Dtos.Requests.SalonOwners;
using Hairhub.Domain.Dtos.Requests.ServiceHairs;
using Hairhub.Domain.Dtos.Requests.Config;
using Hairhub.Domain.Dtos.Requests.Voucher;
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
using Hairhub.Domain.Dtos.Responses.Config;
using Hairhub.Domain.Dtos.Responses.Voucher;
using Hairhub.Domain.Entitities;

namespace Hairhub.Service.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<Region, RegionDto>().ReverseMap();
            CreateMap<CreateAccountRequest, Customer>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.HumanId, opt => opt.MapFrom(src => src.HumanId))
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.BankAccount, opt => opt.MapFrom(src => src.BankAccount))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName));

            CreateMap<CreateAccountRequest, SalonOwner>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.HumanId, opt => opt.MapFrom(src => src.HumanId))
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.BankAccount, opt => opt.MapFrom(src => src.BankAccount))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName));

            CreateMap<CreateAccountResponse, SalonOwner>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.HumanId, opt => opt.MapFrom(src => src.HumanId))
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.BankAccount, opt => opt.MapFrom(src => src.BankAccount))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName));

            CreateMap<CreateAccountResponse, Customer>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.HumanId, opt => opt.MapFrom(src => src.HumanId))
                .ForMember(dest => dest.Img, opt => opt.MapFrom(src => src.Img))
                .ForMember(dest => dest.BankAccount, opt => opt.MapFrom(src => src.BankAccount))
                .ForMember(dest => dest.BankName, opt => opt.MapFrom(src => src.BankName));

            CreateMap<CreateAccountRequest, Account>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));

            CreateMap<CreateAccountResponse, Account>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password));
            CreateMap<UpdateAccountRequest, UpdateAccountRequest>();

            CreateMap<CreateVoucherRequest, Voucher>();
            CreateMap<Voucher, CreateVoucherResponse>();
            CreateMap<UpdateVoucherRequest, Voucher>();
            CreateMap<Voucher, UpdateVoucherResponse>();


            //SalonOwner
            CreateMap<GetSalonOwnerResponse, SalonOwner>().ReverseMap();
            CreateMap<CreateSalonOwnerRequest, SalonOwner>().ReverseMap();
            CreateMap<CreateSalonOwnerResponse, SalonOwner>().ReverseMap();
            CreateMap<UpdateSalonOwnerRequest, SalonOwner>().ReverseMap();

            //SalonEmployee
            CreateMap<GetSalonEmployeeResponse, SalonEmployee>().ReverseMap();
            CreateMap<CreateSalonEmployeeRequest, SalonEmployee>().ReverseMap();
            CreateMap<CreateSalonEmployeeResponse, SalonEmployee>().ReverseMap();
            CreateMap<UpdateSalonEmployeeRequest, SalonEmployee>().ReverseMap();

            //SalonInformation
            CreateMap<GetSalonInformationResponse, SalonInformation>().ReverseMap();
            CreateMap<CreateSalonInformationRequest, SalonInformation>().ReverseMap();
            CreateMap<CreateSalonInformationResponse, SalonInformation>().ReverseMap();
            CreateMap<UpdateSalonInformationRequest, SalonInformation>().ReverseMap();

            //ServiceHair
            CreateMap<GetServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<CreateServiceHairRequest, ServiceHair>().ReverseMap();
            CreateMap<CreateServiceHairResponse, ServiceHair>().ReverseMap();
            CreateMap<UpdateServiceHairRequest, ServiceHair>().ReverseMap();

            //OTP
            CreateMap<SendOtpEmailRequest, OTP>().ReverseMap();
            CreateMap<SendOtpEmailResponse, OTP>().ReverseMap();
            CreateMap<CreateConfigRequest, Config>();
            CreateMap<Config, CreateConfigResponse>();
            CreateMap<UpdateConfigRequest, Config>();
            CreateMap<Config, UpdateConfigResponse>();

        }
    }
}

