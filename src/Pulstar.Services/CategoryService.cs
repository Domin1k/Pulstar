namespace Pulstar.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Constants;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Extensions;
    using Pulstar.Data.Interfaces;
    using Pulstar.DataModels;
    using Pulstar.Models.Category;
    using Pulstar.Services.Interfaces;

    public class CategoryService : ICategoryService
    {
        private readonly IRepository<Category> _categoriesRepository;

        public CategoryService(IRepository<Category> categoriesRepository)
        {
            _categoriesRepository = categoriesRepository;
        }

        public async Task AddCategory(string name, CategoryType catType)
        {
            name.ThrowIfNull();
            var categoryExists = await _categoriesRepository.All().AnyAsync(c => c.Name.ToLower() == name.ToLower());
            if (categoryExists)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.CategoryExists, name));
            }

            var category = new Category
            {
                Name = name,
                Type = catType,
            };

            await _categoriesRepository.AddAsync(category);
        }

        public async Task<IEnumerable<CategoryListingServiceModel>> All(string type)
        {
            var catType = (CategoryType)Enum.Parse(typeof(CategoryType), type);
            return await _categoriesRepository
                .All()
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
            await _categoriesRepository.UpdateAsync(cat);
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
            var category = await _categoriesRepository.All().FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower());
            if (category == null)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.CategoryExists, name));
            }

            return category;
        }
    }
}
