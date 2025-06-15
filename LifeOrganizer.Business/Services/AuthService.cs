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
        private async Task<RefreshToken> GenerateAndStoreRefreshTokenAsync(User user)
        {
            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false,
                CreatedOn = DateTimeOffset.UtcNow,
                IsDeleted = false
            };
            await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            return refreshToken;
        }
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<RefreshTokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
        {
            var refreshTokenRepo = _unitOfWork.Repository<RefreshToken>();
            var token = refreshTokenRequestDto.RefreshToken;
            var refreshToken = await refreshTokenRepo.Query()
                .Where(rt => rt.Token == token && !rt.IsRevoked && !rt.IsUsed && rt.Expires > DateTime.UtcNow)
                .Include(rt => rt.User)
                .FirstOrDefaultAsync();

            if (refreshToken == null)
                return null;

            refreshToken.IsUsed = true;
            refreshToken.IsRevoked = true;
            refreshTokenRepo.Update(refreshToken);

            var user = refreshToken.User;
            if (user == null)
                return null;

            // Generate new tokens
            var newJwt = GenerateJwtToken(user);
            var newRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = user.Id,
                IsRevoked = false,
                IsUsed = false,
                CreatedOn = DateTimeOffset.UtcNow,
                IsDeleted = false
            };
            await refreshTokenRepo.AddAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new RefreshTokenResponseDto
            {
                Token = newJwt,
                RefreshToken = newRefreshToken.Token,
                Username = user.Username,
                Email = user.Email,
                UserId = user.Id
            };
        }

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

            var refreshToken = await GenerateAndStoreRefreshTokenAsync(user);
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                RefreshToken = refreshToken.Token,
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

            var refreshToken = await GenerateAndStoreRefreshTokenAsync(user);
            return new AuthResponseDto
            {
                Token = GenerateJwtToken(user),
                RefreshToken = refreshToken.Token,
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
                expires: DateTime.UtcNow.AddMinutes(5), // Changed to 5 minutes
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
