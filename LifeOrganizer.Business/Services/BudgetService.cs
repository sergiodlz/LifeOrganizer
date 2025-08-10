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
    private readonly IRepository<Transaction> _transactionRepository;

    public BudgetService(IUnitOfWork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _budgetRepository = _unitOfWork.Repository<Budget>();
        _budgetPeriodRepository = _unitOfWork.Repository<BudgetPeriod>();
        _budgetPeriodTransactionRepository = _unitOfWork.Repository<BudgetPeriodTransaction>();
        _transactionRepository = _unitOfWork.Repository<Transaction>();
    }

    public override async Task AddAsync(BudgetDto dto, CancellationToken cancellationToken = default)
    {
        using var dbTransaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var budgetEntity = _mapper.Map<Budget>(dto);
            await _budgetRepository.AddAsync(budgetEntity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var createdBudgetDto = _mapper.Map<BudgetDto>(budgetEntity);
            await EvaluateNewBudgetAgainstExistingTransactionsAsync(createdBudgetDto, cancellationToken);
            await dbTransaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await dbTransaction.RollbackAsync(cancellationToken);
            throw;
        }
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

    private async Task EvaluateNewBudgetAgainstExistingTransactionsAsync(BudgetDto newBudget, CancellationToken cancellationToken)
    {
        var userTransactions = await _transactionRepository.GetAllWithIncludesAsync(newBudget.UserId, t => t.Tags);

        if (!userTransactions.Any())
        {
            return;
        }

        var transactionsByPeriod = userTransactions.GroupBy(t => new { t.OccurredOn.Year, t.OccurredOn.Month });
        var budgetRules = newBudget.Rules;

        foreach (var periodGroup in transactionsByPeriod)
        {
            var year = periodGroup.Key.Year;
            var month = periodGroup.Key.Month;

            var matchingTransactionsInPeriod = new List<Transaction>();
            foreach (var transaction in periodGroup)
            {
                bool matchesRule = budgetRules.Any(rule =>
                    (rule.CategoryId == null || rule.CategoryId == transaction.CategoryId) &&
                    (rule.SubcategoryId == null || rule.SubcategoryId == transaction.SubcategoryId) &&
                    (rule.TagId == null || transaction.Tags.Any(t => t.Id == rule.TagId))
                );

                if (matchesRule)
                {
                    matchingTransactionsInPeriod.Add(transaction);
                }
            }

            if (!matchingTransactionsInPeriod.Any())
            {
                continue;
            }

            var period = await _budgetPeriodRepository.Query()
                .FirstOrDefaultAsync(p => p.BudgetId == newBudget.Id && p.Year == year && p.Month == month, cancellationToken);

            if (period == null)
            {
                period = new BudgetPeriod
                {
                    BudgetId = newBudget.Id,
                    Year = year,
                    Month = month,
                    ActualAmount = 0,
                    UserId = newBudget.UserId
                };
                await _budgetPeriodRepository.AddAsync(period);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            
            decimal periodAmountDelta = 0;
            foreach (var transaction in matchingTransactionsInPeriod)
            {
                var amount = transaction.Type == TransactionType.Income ? -transaction.Amount : transaction.Amount;
                var newLink = new BudgetPeriodTransaction
                {
                    BudgetPeriodId = period.Id,
                    TransactionId = transaction.Id,
                    Amount = amount,
                    UserId = newBudget.UserId
                };
                await _budgetPeriodTransactionRepository.AddAsync(newLink);
                periodAmountDelta += amount;
            }

            period.ActualAmount += periodAmountDelta;
            _budgetPeriodRepository.Update(period);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
