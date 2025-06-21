
using System;
using AutoMapper;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.UnitOfWorkPattern;

namespace LifeOrganizer.Business.Services;

public class CategoryService : GenericService<Category, CategoryDto>, ICategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        : base(unitOfWork, mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CategoryDto> AddWithDefaultSubcategoryAsync(CategoryDto dto, CancellationToken cancellationToken)
    {
        // Map DTO to entity
        var category = _mapper.Map<Category>(dto);

        // Add the category
        await _unitOfWork.Repository<Category>().AddAsync(category);

        // Create default subcategory
        var subcategory = new Subcategory
        {
            Name = "Other",
            CategoryId = category.Id,
            UserId = category.UserId,
            CreatedOn = DateTimeOffset.UtcNow,
            IsDeleted = false
        };
        await _unitOfWork.Repository<Subcategory>().AddAsync(subcategory);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<CategoryDto>(category);
        return result;
    }
}
