namespace LifeOrganizer.Data.Entities;

public class User : BaseEntity
{
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    // Remove large collections to avoid performance issues
    // public ICollection<Category> Categories { get; set; } = new List<Category>();
    // public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    // public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    // public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
