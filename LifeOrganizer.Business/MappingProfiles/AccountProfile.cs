using AutoMapper;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>()
                .ForMember(dest => dest.Pockets, opt => opt.MapFrom(src => src.Pockets));
            CreateMap<AccountDto, Account>()
                .ForMember(dest => dest.Pockets, opt => opt.Ignore()); // Avoid circular mapping
        }
    }
}