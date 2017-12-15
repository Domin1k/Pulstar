namespace Pulstar.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Extensions;
    using Pulstar.Data;
    using Pulstar.Data.Models;
    using Pulstar.Models.Products;
    using Pulstar.Services.Interfaces;

    public class ProductService : IProductService
    {
        private readonly PulstarDbContext _context;
        private readonly UserManager<User> _userManager;

        public ProductService(PulstarDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task AddDiscount(int productId, double discount)
        {
            if (discount <= 0 || discount >= 100)
            {
                throw new InvalidOperationException($"Discount must be in range [1...99]");
            }

            var product = await RetrieveProductOrThrow(productId);

            product.Discount = discount;
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task AddProduct(ProductModel product)
        {
            if (product.ContainsNullStrings())
            {
                throw new InvalidOperationException("Required fields are missing");
            }

            if (!_context.Categories.Any(c => c.Id == product.CategoryId))
            {
                throw new InvalidOperationException($"Category with id {product.CategoryId} does not exist.");
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
            };

            _context.Products.Add(dbEntity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductListingModel>> All(string category)
        {
            var categoryEntity = await _context.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == category.ToLower());

            if (categoryEntity == null)
            {
                throw new InvalidOperationException($"Category {category} does not exists.");
            }

            return await _context
                .Products
                .Where(p => p.CategoryId == categoryEntity.Id)
                .OrderByDescending(p => p.Id)
                .ProjectTo<ProductListingModel>()
                .ToListAsync();
        }

        public async Task EditProduct(int productId, ProductModel product)
        {
            var dbProduct = await RetrieveProductOrThrow(productId);

            ////dbProduct.Price = product.Price;
            ////dbProduct.Quantity = product.Quantity;
            ////dbProduct.Title = product.Title;
            ////dbProduct.Model = product.Model;
            ////dbProduct.Description = product.Description;

            dbProduct = Mapper.Map<Product>(product);

            _context.Products.Update(dbProduct);
            await _context.SaveChangesAsync();
        }

        public async Task<(string category, CategoryType categoryType)> GetCategory(int productId)
        {
            var dbProduct = await _context.Products
                .Where(p => p.Id == productId)
                .Include(p => p.Category)
                .FirstOrDefaultAsync();

            if (dbProduct == null)
            {
                throw new InvalidOperationException($"Product does not exists.");
            }

            return (category: dbProduct.Category.Name, categoryType: dbProduct.Category.Type);
        }

        public async Task<IEnumerable<ProductModel>> List(IEnumerable<int> ids)
        {
            return await _context
                .Products
                .Where(p => ids.Contains(p.Id))
                .OrderByDescending(p => p.Id)
                .ProjectTo<ProductModel>()
                .ToListAsync();
        }

        public async Task<ProductDetailsModel> ViewDetails(int id)
        {
            return await _context
                .Products
                .Where(p => p.Id == id)
                .ProjectTo<ProductDetailsModel>()
                .FirstOrDefaultAsync();
        }

        private async Task<Product> RetrieveProductOrThrow(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId && !p.IsDeleted);

            return product ?? throw new InvalidOperationException($"Product with id {productId} does not exists!");
        }
    }
}
