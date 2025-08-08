using AutoMapper;
using LifeOrganizer.Business.DTOs;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Data.UnitOfWorkPattern;
using Microsoft.EntityFrameworkCore;

namespace LifeOrganizer.Business.Services;

public class BudgetService : GenericService<Budget, BudgetDto>, IBudgetService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IRepository<Budget> _budgetRepository;
    private readonly IRepository<BudgetPeriod> _budgetPeriodRepository;
    private readonly IRepository<BudgetPeriodTransaction> _budgetPeriodTransactionRepository;

    public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _budgetRepository = _unitOfWork.Repository<Budget>();
        _budgetPeriodRepository = _unitOfWork.Repository<BudgetPeriod>();
        _budgetPeriodTransactionRepository = _unitOfWork.Repository<BudgetPeriodTransaction>();
    }

    public async Task EvaluateTransactionAsync(TransactionDto transaction)
    {
        int year = transaction.OccurredOn.Year;
        int month = transaction.OccurredOn.Month;

        var budgets = await _budgetRepository
            .GetAllWithIncludesAsync(transaction.UserId, b => b.Rules);

        foreach (var budget in budgets)
        {
            bool matchesRule = budget.Rules.Any(rule =>
                (rule.CategoryId == null || rule.CategoryId == transaction.CategoryId) &&
                (rule.SubcategoryId == null || rule.SubcategoryId == transaction.SubcategoryId) &&
                (rule.TagId == null || transaction.Tags.Any(t => t.Id == rule.TagId))
            );

            if (!matchesRule) continue;

            var period = await _unitOfWork.Repository<BudgetPeriod>().Query()
                    .FirstOrDefaultAsync(p => p.BudgetId == budget.Id && p.Year == year && p.Month == month);

            if (period == null)
            {
                period = new BudgetPeriod
                {
                    BudgetId = budget.Id,
                    Year = year,
                    Month = month,
                    ActualAmount = 0,
                    UserId = transaction.UserId
                };
                await _budgetPeriodRepository.AddAsync(period);
                await _unitOfWork.SaveChangesAsync();
            }

            var existingLink = await _budgetPeriodTransactionRepository.Query()
                    .AnyAsync(bpt => bpt.BudgetPeriodId == period.Id && bpt.TransactionId == transaction.Id);

            if (existingLink) continue;

            var newLink = new BudgetPeriodTransaction
            {
                BudgetPeriodId = period.Id,
                TransactionId = transaction.Id,
                Amount = transaction.Type == TransactionType.Income ? -transaction.Amount : transaction.Amount,
                UserId = transaction.UserId
            };

            await _budgetPeriodTransactionRepository.AddAsync(newLink);
            period.ActualAmount += newLink.Amount;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ReEvaluateTransactionAsync(TransactionDto oldTransaction, TransactionDto newTransaction)
    {
        await RemoveTransactionEvaluationAsync(oldTransaction);
        await EvaluateTransactionAsync(newTransaction);
    }

    public async Task RemoveTransactionEvaluationAsync(TransactionDto transaction)
    {
        var links = await _budgetPeriodTransactionRepository.Query()
            .Where(bpt => bpt.TransactionId == transaction.Id)
            .Include(bpt => bpt.BudgetPeriod)
            .ToListAsync();

        foreach (var link in links)
        {
            link.BudgetPeriod.ActualAmount -= link.Amount;
            _budgetPeriodTransactionRepository.Remove(link);
        }
        await _unitOfWork.SaveChangesAsync();
    }
}
