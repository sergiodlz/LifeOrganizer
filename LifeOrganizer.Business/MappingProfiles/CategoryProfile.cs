using AutoMapper;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories));
                
            CreateMap<CategoryDto, Category>()
                .ForMember(dest => dest.Subcategories, opt => opt.Ignore()); // Avoid circular mapping
        }
    }
}
