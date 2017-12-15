namespace Pulstar.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Pulstar.Common.Enums;
    using Pulstar.Models.Products;

    public interface IProductService
    {
        Task<IEnumerable<ProductListingModel>> All(string category);

        Task<IEnumerable<ProductModel>> List(IEnumerable<int> ids);

        Task<ProductDetailsModel> ViewDetails(int id);

        Task AddProduct(ProductModel product);

        Task EditProduct(int productId, ProductModel product);

        Task AddDiscount(int productId, double discount);

        Task<(string category, CategoryType categoryType)> GetCategory(int productId);
    }
}
