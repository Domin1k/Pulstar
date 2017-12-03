namespace Pulstar.Data.Models
{
    using System;

    public abstract class BaseEntity<TKey>
    {
        public TKey Id { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}
