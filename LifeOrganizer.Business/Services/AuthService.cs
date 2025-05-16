using LifeOrganizer.Business.DTOs.Auth;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.UnitOfWorkPattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LifeOrganizer.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            // Use Guid.Empty to bypass userId filter for authentication
            var user = await _unitOfWork.Repository<User>()
                .Query()
                .Where(u => !u.IsDeleted && (u.Username == loginDto.UsernameOrEmail || u.Email == loginDto.UsernameOrEmail))
                .FirstOrDefaultAsync();

            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                return null;

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Email = user.Email,
                UserId = user.Id
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto)
        {
            var userRepo = _unitOfWork.Repository<User>();
            if (await userRepo.Query().AnyAsync(u => u.Username == registerDto.Username || u.Email == registerDto.Email))
                return null;

            var user = new User
            {
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                IsDeleted = false
            };
            await userRepo.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Create a default account for the new user
            var accountRepo = _unitOfWork.Repository<Account>();
            var account = new Account
            {
                Name = "Efectivo",
                Type = AccountType.Cash,
                Currency = CurrencyType.COP,
                IncludeInGlobalBalance = true,
                UserId = user.Id,
                IsDeleted = false,
                CreatedOn = DateTimeOffset.UtcNow
            };
            await accountRepo.AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            // Create default categories for the new user
            var defaultCategories = new[] { "Alimentación", "Transporte", "Salud", "Educación", "Entretenimiento", "Vivienda", "Otros" };
            var categoryRepo = _unitOfWork.Repository<Category>();
            foreach (var categoryName in defaultCategories)
            {
                var category = new Category
                {
                    Name = categoryName,
                    UserId = user.Id,
                    IsDeleted = false,
                    CreatedOn = DateTimeOffset.UtcNow
                };
                await categoryRepo.AddAsync(category);
            }
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Email = user.Email,
                UserId = user.Id
            };
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(12),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
