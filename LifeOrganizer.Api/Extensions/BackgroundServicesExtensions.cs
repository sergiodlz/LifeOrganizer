using LifeOrganizer.Api.BackgroundServices;

namespace LifeOrganizer.Api.Extensions;

public static class BackgroundServicesExtensions
{
    public static IServiceCollection AddBackgroundWorkers(this IServiceCollection services)
    {
        services.AddHostedService<AccountBalanceUpdateWorker>();
        return services;
    }
}
