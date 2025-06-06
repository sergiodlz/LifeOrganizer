using LifeOrganizer.Business.DTOs.Auth;
using LifeOrganizer.Data.Entities;
using LifeOrganizer.Data.UnitOfWorkPattern;
using LifeOrganizer.Shared.Resources;
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

        // Removed duplicate constructor

        public async Task<AuthResponseDto?> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _unitOfWork.Repository<User>()
                .Query()
                .Where(u => u.Id == userId && !u.IsDeleted)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
                return null;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                Username = user.Username,
                Email = user.Email,
                UserId = user.Id
            };
        }

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

            await CreateDefaultAccountAsync(user.Id);
            await CreateDefaultCategoriesAsync(user.Id);
            
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

        private async Task CreateDefaultAccountAsync(Guid userId)
        {
            var account = new Account
            {
                Name = SharedResources.DefaultAccountName,
                Type = AccountType.Cash,
                Currency = CurrencyType.COP,
                IncludeInGlobalBalance = true,
                UserId = userId,
                IsDeleted = false,
                CreatedOn = DateTimeOffset.UtcNow
            };
            await _unitOfWork.Repository<Account>().AddAsync(account);
        }

        private async Task CreateDefaultCategoriesAsync(Guid userId)
        {
            var categoryRepo = _unitOfWork.Repository<Category>();
            var categories = SharedResources.DefaultCategories
                .Select(name => new Category
                {
                    Name = name,
                    UserId = userId,
                    IsDeleted = false,
                    CreatedOn = DateTimeOffset.UtcNow
                })
                .ToList();

            await categoryRepo.AddRangeAsync(categories);
        }
    }
}
