using AutoMapper;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>().ReverseMap();
        }
    }
}