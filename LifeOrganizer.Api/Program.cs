using Microsoft.EntityFrameworkCore;
using LifeOrganizer.Data.UnitOfWorkPattern;
using LifeOrganizer.Data.Repositories;
using LifeOrganizer.Business.Services;
using LifeOrganizer.Business.MappingProfiles;
using LifeOrganizer.Data;
using FluentValidation.AspNetCore;
using FluentValidation;
using LifeOrganizer.Business.Validators;
using LifeOrganizer.Api.Middleware;
using LifeOrganizer.Api.Extensions;
using LifeOrganizer.Api.BackgroundServices;
using LifeOrganizer.Api.Services;

var builder = WebApplication.CreateBuilder(args);
// FluentValidation registration
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<AccountDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TransactionDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TagDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SubcategoryDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CategoryDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateUserDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PocketDtoValidator>();
// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Configure PostgreSQL with EF Core
builder.Services.AddDbContext<LifeOrganizer.Data.LifeOrganizerContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register UnitOfWork and generic Repository for DI
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPocketTransactionService, PocketTransactionService>();

// Add controllers
builder.Services.AddControllers();

// Add JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer";
    options.DefaultChallengeScheme = "JwtBearer";
})
.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)),
        ClockSkew = TimeSpan.Zero
    };
});

// Register AuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add health checks
builder.Services.AddHealthChecks();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(AccountProfile).Assembly);
builder.Services.AddAutoMapper(typeof(CategoryProfile).Assembly);
builder.Services.AddAutoMapper(typeof(SubcategoryProfile).Assembly);
builder.Services.AddAutoMapper(typeof(TagProfile).Assembly);
builder.Services.AddAutoMapper(typeof(TransactionProfile).Assembly);
builder.Services.AddAutoMapper(typeof(PocketProfile).Assembly);
builder.Services.AddAutoMapper(typeof(PocketTransactionProfile).Assembly);

// Register services and background workers
builder.Services.AddScoped<IAccountBalanceService, AccountBalanceService>();
builder.Services.AddBackgroundWorkers();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Run database migrations at startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LifeOrganizerContext>();
    db.Database.Migrate();

    // Run initial balance update
    var balanceService = scope.ServiceProvider.GetRequiredService<IAccountBalanceService>();
    await balanceService.UpdateAllBalancesAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

// Enable CORS
app.UseCors("AllowAll");

// Enable authentication/authorization
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Enable attribute-routed controllers
app.MapControllers();

// Add health checks endpoint
app.MapHealthChecks("/health");

app.Run();
