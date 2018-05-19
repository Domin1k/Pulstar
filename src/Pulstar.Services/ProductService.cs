namespace Pulstar.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Constants;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Extensions;
    using Pulstar.Data.Interfaces;
    using Pulstar.DataModels;
    using Pulstar.Services.Interfaces;
    using Pulstar.Services.Models.Products;

    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoriesRepository;
        private readonly UserManager<User> _userManager;

        public ProductService(IRepository<Product> productRepository, IRepository<Category> categoriesRepository, UserManager<User> userManager)
        {
            _productRepository = productRepository;
            _categoriesRepository = categoriesRepository;
            _userManager = userManager;
        }

        public async Task AddDiscount(int productId, double discount)
        {
            if (discount <= 0 || discount >= 100)
            {
                throw new InvalidOperationException(ServiceErrorsConstants.InvalidDiscountRange);
            }

            var product = await RetrieveProductOrThrow(productId);

            product.Discount = discount;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        public async Task AddProduct(ProductModel product)
        {
            if (product.ContainsNullStrings())
            {
                throw new InvalidOperationException(ServiceErrorsConstants.RequiredFields);
            }

            if (!_categoriesRepository.All().Any(c => c.Id == product.CategoryId))
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.CategoryDoesNotExist, product.CategoryId));
            }

            var dbEntity = new Product
            {
                CategoryId = product.CategoryId,
                Manufacturer = product.Manufacturer,
                Model = product.Model,
                Title = product.Title,
                Description = product.Description,
                Quantity = product.Quantity,
                Price = product.Price,
                Discount = product.Discount,
                PurchaseId = 1,
            };

            _productRepository.Add(dbEntity);
            await _productRepository.SaveChangesAsync();
            var ent = _productRepository.All().ToList();
        }

        public async Task<IEnumerable<ProductListingModel>> All(string category, Expression<Func<Product, object>> orderPredicate, OrderType orderType)
        {
            var categoryEntity = await _categoriesRepository.All().FirstOrDefaultAsync(c => c.Name.ToLower() == category.ToLower());

            if (categoryEntity == null)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.CategoryDoesNotExist, category));
            }

            var products = _productRepository
                .All()
                .Where(p => p.CategoryId == categoryEntity.Id && !p.IsDeleted)
                .AsQueryable();

            return await ListProducts(products, orderPredicate, orderType);
        }

        public async Task<IEnumerable<ProductListingModel>> All(Expression<Func<Product, bool>> wherePredicate, Expression<Func<Product, object>> orderPredicate, OrderType orderType, int take)
        {
            var products = _productRepository
                .All()
                .Where(p => !p.IsDeleted)
                .AsQueryable();
            if (wherePredicate != null)
            {
                products = products.Where(wherePredicate);
            }

            products = products.Take(take).AsQueryable();
            return await ListProducts(products, orderPredicate, orderType);
        }

        public async Task EditProduct(int productId, ProductModel product, byte[] image)
        {
            var dbProduct = await RetrieveProductOrThrow(productId);

            dbProduct.Price = product.Price;
            dbProduct.Quantity = product.Quantity;
            dbProduct.Title = product.Title;
            dbProduct.Model = product.Model;
            dbProduct.Manufacturer = product.Manufacturer;
            dbProduct.Discount = product.Discount;
            dbProduct.Description = product.Description;
            dbProduct.Image = image;
            dbProduct.ModifiedOn = DateTime.UtcNow;

            _productRepository.Add(dbProduct);
            await _productRepository.SaveChangesAsync();
        }

        public async Task<(string category, CategoryType categoryType)> GetCategory(int productId)
        {
            var dbProduct = await _productRepository
                .All()
                .Where(p => p.Id == productId)
                .Include(p => p.Category)
                .FirstOrDefaultAsync();

            if (dbProduct == null)
            {
                throw new InvalidOperationException(ServiceErrorsConstants.ProductDoesNotExist);
            }

            return (category: dbProduct.Category.Name, categoryType: dbProduct.Category.Type);
        }

        public async Task<IEnumerable<ProductModel>> List(IEnumerable<int> ids)
        {
            return await _productRepository
                .All()
                .Where(p => ids.Contains(p.Id))
                .OrderByDescending(p => p.Id)
                .ProjectTo<ProductModel>()
                .ToListAsync();
        }

        public async Task<ProductDetailsModel> ViewDetails(int id)
        {
            return await _productRepository
                .All()
                .Where(p => p.Id == id && !p.IsDeleted)
                .ProjectTo<ProductDetailsModel>()
                .FirstOrDefaultAsync();
        }

        public async Task DeleteProduct(int productId)
        {
            var product = await RetrieveProductOrThrow(productId);

            product.IsDeleted = true;
            product.DeletedOn = DateTime.UtcNow;
            _productRepository.Update(product);
            await _productRepository.SaveChangesAsync();
        }

        private async Task<Product> RetrieveProductOrThrow(int productId)
        {
            var product = await _productRepository.All().FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

            return product ?? throw new InvalidOperationException(string.Format(ServiceErrorsConstants.ProductIdDoesNotExist, productId));
        }

        private async Task<IEnumerable<ProductListingModel>> ListProducts(IQueryable<Product> products, Expression<Func<Product, object>> orderPredicate, OrderType orderType)
        {
            if (orderType == OrderType.Ascending)
            {
                products = products.OrderBy(orderPredicate);
            }
            else if (orderType == OrderType.Descending)
            {
                products = products.OrderByDescending(orderPredicate);
            }

            return await products.ProjectTo<ProductListingModel>().ToListAsync();
        }
    }
}
