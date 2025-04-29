namespace LifeOrganizer.Data.Entities;

public class User : BaseEntity
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FullName { get; set; }
    // Only keep navigation properties for small or frequently traversed collections
    public ICollection<Account> Accounts { get; set; } = new List<Account>();
    // Remove large collections to avoid performance issues
    // public ICollection<Category> Categories { get; set; } = new List<Category>();
    // public ICollection<Subcategory> Subcategories { get; set; } = new List<Subcategory>();
    // public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    // public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
