using System;

namespace LifeOrganizer.Data.Entities;

public class PocketTransaction : BaseTransaction
{
    public Guid PocketId { get; set; }
    public Pocket? Pocket { get; set; }
}
