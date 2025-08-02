using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Business.MappingProfiles;

public class BudgetRuleProfile: Profile
{
    public BudgetRuleProfile()
    {
        CreateMap<BudgetRule, BudgetRuleDto>().ReverseMap();
    }
}
