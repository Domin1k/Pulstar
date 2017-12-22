namespace Pulstar.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Extensions;
    using Pulstar.Data;
    using Pulstar.Data.Models;
    using Pulstar.Models.Category;
    using Pulstar.Services.Interfaces;

    public class CategoryService : ICategoryService
    {
        private readonly PulstarDbContext _dbContext;

        public CategoryService(PulstarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddCategory(string name, CategoryType catType)
        {
            name.ThrowIfNull();
            await RetrieveOrThrow(name);

            var category = new Category
            {
                Name = name,
                Type = catType,
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CategoryListingServiceModel>> All(string type)
        {
            var catType = (CategoryType)Enum.Parse(typeof(CategoryType), type);
            return await _dbContext
                .Categories
                .Where(c => c.Type == catType && !c.IsDeleted)
                .OrderByDescending(c => c.Id)
                .ProjectTo<CategoryListingServiceModel>()
                .ToListAsync();
        }

        public async Task DeleteCategory(string name)
        {
            var cat = await RetrieveOrThrow(name);

            cat.IsDeleted = true;
            cat.DeletedOn = DateTime.UtcNow;
            _dbContext.Categories.Update(cat);
            await _dbContext.SaveChangesAsync();
        }

        public Task<CategoryDetailsModel> Details(string categoryName)
        {
            throw new NotImplementedException();
        }

        public Task EditCategory(string name, CategoryType catType)
        {
            throw new NotImplementedException();
        }
        
        private async Task<Category> RetrieveOrThrow(string name)
        {
            var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            if (category == null)
            {
                throw new InvalidOperationException($"Category {name} already exists!");
            }

            return category;
        }
    }
}
