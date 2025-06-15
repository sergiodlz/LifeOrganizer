namespace LifeOrganizer.Business.DTOs.Auth
{
    public class RefreshTokenRequestDto
    {
        public required string RefreshToken { get; set; }
    }

    public class RefreshTokenResponseDto
    {
        public required string Token { get; set; }
        public required string RefreshToken { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required Guid UserId { get; set; }
    }
}
