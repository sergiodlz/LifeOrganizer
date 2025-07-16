using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles;

public class PocketTransactionProfile : Profile
{
    public PocketTransactionProfile()
    {
        CreateMap<PocketTransaction, PocketTransactionDto>()
            .ForMember(dest => dest.PocketName, opt => opt.MapFrom(src => src.Pocket != null ? src.Pocket.Name : null));

        CreateMap<PocketTransactionDto, PocketTransaction>()
            .ForMember(dest => dest.Pocket, opt => opt.Ignore());
    }
}
