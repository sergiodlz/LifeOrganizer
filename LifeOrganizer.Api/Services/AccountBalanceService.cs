using LifeOrganizer.Data.UnitOfWorkPattern;
using Microsoft.EntityFrameworkCore;
using LifeOrganizer.Data.Entities;

namespace LifeOrganizer.Api.Services;

public class AccountBalanceService : IAccountBalanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AccountBalanceService> _logger;

    public AccountBalanceService(
        IUnitOfWork unitOfWork,
        ILogger<AccountBalanceService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task UpdateAllBalancesAsync()
    {
        _logger.LogInformation("Starting account balance update at {time}", DateTimeOffset.Now);

        // Get all non-deleted accounts
        var accounts = await _unitOfWork.Repository<Account>()
            .Query()
            .Where(a => !a.IsDeleted)
            .ToListAsync();

        foreach (var account in accounts)
        {
            try
            {
                // Calculate balance from all non-deleted transactions
                var balance = await _unitOfWork.Repository<Transaction>()
                    .Query()
                    .Where(t => t.AccountId == account.Id && !t.IsDeleted)
                    .SumAsync(t => t.Type == TransactionType.Income ? t.Amount : -t.Amount);

                account.Balance = balance;
                _unitOfWork.Repository<Account>().Update(account);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Updated balance for account {accountId} to {balance}", account.Id, balance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating balance for account {accountId}", account.Id);
            }
        }

        _logger.LogInformation("Completed account balance update at {time}", DateTimeOffset.Now);
    }
}
