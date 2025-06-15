using System;

namespace LifeOrganizer.Data.Entities
{
    public class RefreshToken : BaseEntity
    {
        public required string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }
        public bool IsUsed { get; set; }
        public new Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
