namespace Pulstar.Data.Interfaces
{
    using System;

    public interface IEntity
    {
        DateTime? CreatedOn { get; set; }

        DateTime? ModifiedOn { get; set; }

        DateTime? DeletedOn { get; set; }        

        bool IsDeleted { get; set; }
    }
}
