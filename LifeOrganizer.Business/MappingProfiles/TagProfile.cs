using AutoMapper;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}
