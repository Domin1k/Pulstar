namespace Pulstar.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pulstar.Models.Category;

    public interface ICategoryService
    {
        Task<IEnumerable<CategoryListingServiceModel>> All(string type);
    }
}
