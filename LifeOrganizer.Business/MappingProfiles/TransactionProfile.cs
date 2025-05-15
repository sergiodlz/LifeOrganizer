using AutoMapper;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Transaction, TransactionDto>().ReverseMap();
        }
    }
}
