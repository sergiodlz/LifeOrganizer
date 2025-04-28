namespace LifeOrganizer.Data.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public Guid UserId { get; set; } // User Ownership
    public bool IsDeleted { get; set; } // Soft Delete
}
