using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeOrganizer.Data.Entities
{
    public class Pocket : BaseEntity
    {
        public required string Name { get; set; }
        public Guid AccountId { get; set; }
        public required Account Account { get; set; }
        public decimal Balance { get; set; }
    }
}
