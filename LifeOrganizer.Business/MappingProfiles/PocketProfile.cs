using AutoMapper;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Business.MappingProfiles;

public class PocketProfile : Profile
{
    public PocketProfile()
    {
        CreateMap<Pocket, PocketDto>().ReverseMap();
    }
}
