using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using LifeOrganizer.Data.UnitOfWorkPattern;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Business.Services;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
ConfigureServices(builder.Services, builder.Configuration, builder.Logging);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigurePipeline(app);

app.Run();

static void ConfigureServices(IServiceCollection services, IConfiguration configuration, ILoggingBuilder logging)
{
    // Configure logging
    logging.ClearProviders();
    logging.AddConsole();

    // Configure PostgreSQL with EF Core
    services.AddDbContext<LifeOrganizer.Data.LifeOrganizerContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    // Register UnitOfWork and generic Repository for DI
    services.AddScoped<IUnitOfWork, UnitOfWork>();
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddScoped(typeof(IGenericService<>), typeof(GenericService<>));

    // Add controllers
    services.AddControllers();

    // Add CORS
    services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
    });

    // Add health checks
    services.AddHealthChecks();

    // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
    services.AddOpenApi();
}

static void ConfigurePipeline(WebApplication app)
{
    // Run database migrations at startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<LifeOrganizer.Data.LifeOrganizerContext>();
        db.Database.Migrate();
    }

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/error");
        app.UseHsts();
    }

    // Enable CORS
    app.UseCors("AllowAll");

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    // Enable attribute-routed controllers
    app.MapControllers();

    // Add health checks endpoint
    app.MapHealthChecks("/health");
}
