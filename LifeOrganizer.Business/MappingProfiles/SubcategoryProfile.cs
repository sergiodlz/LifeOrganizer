using AutoMapper;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class SubcategoryProfile : Profile
    {
        public SubcategoryProfile()
        {
            CreateMap<Subcategory, SubcategoryDto>().ReverseMap();
        }
    }
}
