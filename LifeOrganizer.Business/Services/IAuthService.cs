using LifeOrganizer.Business.DTOs.Auth;

namespace LifeOrganizer.Business.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto);
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
        Task<RefreshTokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
    }
}
