namespace Pulstar.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<CategoryListingServiceModel>> All()
        {
            return await _dbContext
                .Categories
                .OrderByDescending(c => c.Id)
                .ProjectTo<CategoryListingServiceModel>()
                .ToListAsync();
        }
    }
}
