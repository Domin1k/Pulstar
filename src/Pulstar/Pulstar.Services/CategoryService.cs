namespace Pulstar.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Enums;
    using Pulstar.Data;
    using Pulstar.Models.Category;
    using Pulstar.Services.Interfaces;

    public class CategoryService : ICategoryService
    {
        private readonly PulstarDbContext _dbContext;

        public CategoryService(PulstarDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CategoryListingServiceModel>> All(string type)
        {
            var catType = (CategoryType)Enum.Parse(typeof(CategoryType), type);
            return await _dbContext
                .Categories
                .Where(c => c.Type == catType)
                .OrderByDescending(c => c.Id)
                .ProjectTo<CategoryListingServiceModel>()
                .ToListAsync();
        }
    }
}
