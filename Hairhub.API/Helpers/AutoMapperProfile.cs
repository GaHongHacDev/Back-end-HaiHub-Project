using AutoMapper;
using Hairhub.Domain.Dtos.Requests.Accounts;
using Hairhub.Domain.Dtos.Responses.Accounts;
using Hairhub.Domain.Entitities;

namespace Hairhub.API.Helpers
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
            CreateMap<UpdateAccountRequest, UpdateAccountResponse>();
        }
    }
}
