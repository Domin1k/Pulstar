namespace Pulstar.DataModels
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using Pulstar.Data.Interfaces;

    public abstract class BaseEntity : IEntity
    {
        [Key]
        public int Id { get; set; }

        public DateTime? CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public bool IsDeleted { get; set; }
    }
}
