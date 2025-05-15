using AutoMapper;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;

namespace LifeOrganizer.Business.MappingProfiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
