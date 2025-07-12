using LifeOrganizer.Api.Services;

namespace LifeOrganizer.Api.BackgroundServices;

public class AccountBalanceUpdateWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountBalanceUpdateWorker> _logger;

    public AccountBalanceUpdateWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<AccountBalanceUpdateWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await UpdateBalances();

                // Calculate time until next run (2 AM tomorrow)
                var now = DateTimeOffset.Now;
                var next2AM = now.Date.AddDays(1).AddHours(2);
                if (now.Hour >= 2)
                {
                    next2AM = next2AM.AddDays(1);
                }

                var delay = next2AM - now;
                await Task.Delay(delay, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating account balances");
                // Wait for 5 minutes before retrying in case of error
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task UpdateBalances()
    {
        using var scope = _scopeFactory.CreateScope();
        var balanceService = scope.ServiceProvider.GetRequiredService<IAccountBalanceService>();
        await balanceService.UpdateAllBalancesAsync();
    }
}
