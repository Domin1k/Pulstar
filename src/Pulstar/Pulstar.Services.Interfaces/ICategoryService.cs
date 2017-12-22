namespace Pulstar.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pulstar.Common.Enums;
    using Pulstar.Models.Category;

    public interface ICategoryService
    {
        Task<CategoryDetailsModel> Details(string categoryName);

        Task AddCategory(string name, CategoryType catType);

        Task DeleteCategory(string name);

        Task EditCategory(string name, CategoryType catType);

        Task<IEnumerable<CategoryListingServiceModel>> All(string type);
    }
}
