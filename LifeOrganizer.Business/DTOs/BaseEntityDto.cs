using System;

namespace LifeOrganizer.Business.DTOs;

public class BaseEntityDto
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public Guid UserId { get; set; }
    public bool IsDeleted { get; set; }
}
