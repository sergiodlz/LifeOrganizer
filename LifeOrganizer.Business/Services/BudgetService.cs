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

    public async Task EvaluateTransactionAsync(TransactionDto transactionDto)
    {
        var transaction = _mapper.Map<Transaction>(transactionDto);
        var budgets = await _budgetRepository
            .GetAllWithIncludesAsync(transaction.UserId, b => b.Rules);

        var matchingBudgets = budgets
            .Where(budget =>
                DoesTransactionMatchRules(transaction, _mapper.Map<ICollection<BudgetRuleDto>>(budget.Rules)))
            .ToList();

        if (!matchingBudgets.Any()) return;

        var budgetIds = matchingBudgets.Select(b => b.Id).ToList();
        var periodsByBudgetId = await GetOrCreatePeriodsForTransactionAsync(budgetIds, transactionDto);
        var existingLinkedPeriodIds = await _budgetPeriodTransactionRepository.Query()
            .Where(bpt => bpt.TransactionId == transaction.Id)
            .Select(bpt => bpt.BudgetPeriodId)
            .ToHashSetAsync();

        var newLinksToAdd = new List<BudgetPeriodTransaction>();
        foreach (var budget in matchingBudgets)
        {
            var period = periodsByBudgetId[budget.Id];
            if (existingLinkedPeriodIds.Contains(period.Id)) continue;

            var amount = CalculateBudgetAmount(transaction, budget.Currency);
            if (amount == 0) continue;

            newLinksToAdd.Add(new BudgetPeriodTransaction
            {
                BudgetPeriodId = period.Id,
                TransactionId = transaction.Id,
                Amount = amount,
                UserId = transaction.UserId
            });

            period.ActualAmount += amount;
            _budgetPeriodRepository.Update(period);
        }

        if (newLinksToAdd.Any())
        {
            await _budgetPeriodTransactionRepository.AddRangeAsync(newLinksToAdd);
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
        if (!userTransactions.Any()) return;

        var matchingTransactionsByPeriod = userTransactions
            .Where(t => DoesTransactionMatchRules(t, newBudget.Rules))
            .GroupBy(t => (t.OccurredOn.Year, t.OccurredOn.Month))
            .ToDictionary(g => g.Key, g => g.ToList());
        if (matchingTransactionsByPeriod.Count == 0) return;

        var budgetPeriods = await GetOrCreateBudgetPeriodsAsync(newBudget, matchingTransactionsByPeriod.Keys, cancellationToken);

        var allNewLinks = new List<BudgetPeriodTransaction>();
        foreach (var periodGroup in matchingTransactionsByPeriod)
        {
            var period = budgetPeriods[periodGroup.Key];
            var transactionsInPeriod = periodGroup.Value;

            var newLinks = transactionsInPeriod
                .Select(transaction => new { transaction, amount = CalculateBudgetAmount(transaction, newBudget.Currency) })
                .Where(t => t.amount != 0)
                .Select(t => new BudgetPeriodTransaction
                {
                    BudgetPeriodId = period.Id,
                    TransactionId = t.transaction.Id,
                    Amount = t.amount,
                    UserId = newBudget.UserId
                }).ToList();

            if (newLinks.Count > 0)
            {
                allNewLinks.AddRange(newLinks);
                period.ActualAmount += newLinks.Sum(link => link.Amount);
                _budgetPeriodRepository.Update(period);
            }
        }

        if (allNewLinks.Count != 0)
        {
            await _budgetPeriodTransactionRepository.AddRangeAsync(allNewLinks);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private decimal CalculateBudgetAmount(Transaction transaction, CurrencyType budgetCurrency)
    {
        decimal amount;
        if (budgetCurrency == transaction.Currency)
        {
            amount = transaction.Amount;
        }
        else if (transaction.ExchangeAmount.HasValue && transaction.ExchangeAmount.Value > 0)
        {
            amount = transaction.ExchangeAmount.Value;
        }
        else
        {
            return 0;
        }

        return transaction.Type == TransactionType.Income ? -amount : amount;
    }

    private bool DoesTransactionMatchRules(Transaction transaction, ICollection<BudgetRuleDto> rules)
    {
        return rules.Any(rule =>
            (rule.CategoryId == null || rule.CategoryId == transaction.CategoryId) &&
            (rule.SubcategoryId == null || rule.SubcategoryId == transaction.SubcategoryId) &&
            (rule.TagId == null || (transaction.Tags != null && transaction.Tags.Any(t => t.Id == rule.TagId)))
        );
    }

    private async Task<Dictionary<(int Year, int Month), BudgetPeriod>> GetOrCreateBudgetPeriodsAsync(
    BudgetDto budget,
    IEnumerable<(int Year, int Month)> requiredPeriods,
    CancellationToken cancellationToken)
    {
        var existingPeriods = (await _budgetPeriodRepository.Query()
            .Where(p => p.BudgetId == budget.Id)
            .ToListAsync(cancellationToken))
            .ToDictionary(p => (p.Year, p.Month));

        var newPeriodsToCreate = new List<BudgetPeriod>();
        foreach (var periodKey in requiredPeriods)
        {
            if (!existingPeriods.ContainsKey(periodKey))
            {
                var newPeriod = new BudgetPeriod
                {
                    BudgetId = budget.Id,
                    Year = periodKey.Year,
                    Month = periodKey.Month,
                    ActualAmount = 0,
                    UserId = budget.UserId
                };
                newPeriodsToCreate.Add(newPeriod);
                existingPeriods.Add(periodKey, newPeriod);
            }
        }

        if (newPeriodsToCreate.Count != 0)
        {
            await _budgetPeriodRepository.AddRangeAsync(newPeriodsToCreate);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return existingPeriods;
    }
    
    private async Task<Dictionary<Guid, BudgetPeriod>> GetOrCreatePeriodsForTransactionAsync(List<Guid> budgetIds, TransactionDto transaction)
    {
        int year = transaction.OccurredOn.Year;
        int month = transaction.OccurredOn.Month;

        var existingPeriods = await _budgetPeriodRepository.Query()
            .Where(p => budgetIds.Contains(p.BudgetId) && p.Year == year && p.Month == month)
            .ToDictionaryAsync(p => p.BudgetId);

        var newPeriodsToCreate = new List<BudgetPeriod>();
        foreach (var budgetId in budgetIds)
        {
            if (!existingPeriods.ContainsKey(budgetId))
            {
                var newPeriod = new BudgetPeriod
                {
                    BudgetId = budgetId,
                    Year = year,
                    Month = month,
                    ActualAmount = 0,
                    UserId = transaction.UserId
                };
                newPeriodsToCreate.Add(newPeriod);
                existingPeriods.Add(budgetId, newPeriod);
            }
        }

        if (newPeriodsToCreate.Any())
        {
            await _budgetPeriodRepository.AddRangeAsync(newPeriodsToCreate);
            await _unitOfWork.SaveChangesAsync();
        }

        return existingPeriods;
    }
}
