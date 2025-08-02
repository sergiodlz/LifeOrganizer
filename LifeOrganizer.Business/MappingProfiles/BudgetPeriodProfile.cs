using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles;

public class BudgetPeriodProfile : Profile
{
    public BudgetPeriodProfile()
    {
        CreateMap<BudgetPeriod, BudgetPeriodDto>().ReverseMap();
    }
}