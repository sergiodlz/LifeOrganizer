using System.Security.Claims;

namespace LifeOrganizer.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal user)
        {
            var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? user.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
            return sub != null && Guid.TryParse(sub, out var guid) ? guid : throw new UnauthorizedAccessException("Invalid user id in token");
        }
    }
}
