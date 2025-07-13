using AutoMapper;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Account != null ? src.Account.Name : null));

            CreateMap<TransactionDto, Transaction>()
                .ForMember(dest => dest.Account, opt => opt.Ignore())
                .ForMember(dest => dest.Category, opt => opt.Ignore())
                .ForMember(dest => dest.Subcategory, opt => opt.Ignore());
        }
    }
}
