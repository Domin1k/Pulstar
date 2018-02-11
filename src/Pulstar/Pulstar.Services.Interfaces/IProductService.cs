namespace Pulstar.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Pulstar.Common.Enums;
    using Pulstar.Data.Models;
    using Pulstar.Models.Products;

    public interface IProductService
    {
        Task<IEnumerable<ProductListingModel>> All(Expression<Func<Product, bool>> wherePredicate, Expression<Func<Product, object>> orderPredicate, OrderType orderType, int take);

        Task<IEnumerable<ProductListingModel>> All(string category, Expression<Func<Product, object>> orderPredicate, OrderType orderType);

        Task<IEnumerable<ProductModel>> List(IEnumerable<int> ids);

        Task<ProductDetailsModel> ViewDetails(int id);

        Task AddProduct(ProductModel product);

        Task EditProduct(int productId, ProductModel product, byte[] image);

        Task DeleteProduct(int productId);

        Task AddDiscount(int productId, double discount);

        Task<(string category, CategoryType categoryType)> GetCategory(int productId);
    }
}
