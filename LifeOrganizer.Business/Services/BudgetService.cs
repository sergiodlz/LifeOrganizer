using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.UnitOfWorkPattern;

namespace LifeOrganizer.Business.Services;

public class BudgetService : GenericService<Budget, BudgetDto>, IBudgetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<Budget> _budgetRepository;
    private readonly IRepository<BudgetPeriod> _budgetPeriodRepository;

    public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _budgetRepository = _unitOfWork.Repository<Budget>();
        _budgetPeriodRepository = _unitOfWork.Repository<BudgetPeriod>();
    }

    public async Task EvaluateTransactionAsync(TransactionDto transaction)
    {
        int year = transaction.OccurredOn.Year;
        int month = transaction.OccurredOn.Month;

        // 1️⃣ Load all budgets with rules and periods for this user
        var budgets = await _budgetRepository
            .GetAllWithIncludesAsync(transaction.UserId, b => b.Rules, b => b.Periods);

        foreach (var budget in budgets)
        {
            bool matchesRule = budget.Rules.Any(rule =>
                (rule.CategoryId == null || rule.CategoryId == transaction.CategoryId) &&
                (rule.SubcategoryId == null || rule.SubcategoryId == transaction.SubcategoryId) &&
                (rule.TagId == null || transaction.Tags.Any(t => t.Id == rule.TagId))
            );

            if (!matchesRule) continue;

            // 2️⃣ Find or create the BudgetPeriod for current month
            var period = budget.Periods
                .FirstOrDefault(p => p.Month == month && p.Year == year);

            if (period == null)
            {
                period = new BudgetPeriod
                {
                    Id = Guid.NewGuid(),
                    BudgetId = budget.Id,
                    Year = year,
                    Month = month,
                    ActualAmount = 0
                };

                await _budgetPeriodRepository.AddAsync(period);
            }

            // 3️⃣ Update ActualAmount
            period.ActualAmount -= transaction.Type == TransactionType.Income ? transaction.Amount : -transaction.Amount;
        }

        // 4️⃣ Commit changes through Unit of Work
        await _unitOfWork.SaveChangesAsync();
    }

}
