namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Ploeh.AutoFixture;
    using Pulstar.Data;
    using Pulstar.Data.Interfaces;
    using Pulstar.DataModels;
    using Pulstar.Services.Models.Products;
    using Pulstar.Web.Infrastructure;

    public abstract class BaseTest
    {
        public const string DefaultCategoryName = "test-category";

        static BaseTest()
        {
            Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
        }
        
        protected PulstarDbContext GetDatabase()
        {
            var dbOptions = new DbContextOptionsBuilder<PulstarDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            return new PulstarDbContext(dbOptions);
        }

        protected async Task<ProductModel> GenerateValidProduct(IRepository<Category> categoryRepo)
        {
            var product = new Fixture().Create<ProductModel>();
            var category = new Category { Name = DefaultCategoryName };
            categoryRepo.Add(category);
            await categoryRepo.SaveChangesAsync();
            product.CategoryId = category.Id;
            return product;
        }
    }
}
