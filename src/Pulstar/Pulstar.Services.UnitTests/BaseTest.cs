namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using AutoMapper;
    using Microsoft.EntityFrameworkCore;
    using Ploeh.AutoFixture;
    using Pulstar.Data;
    using Pulstar.Data.Models;
    using Pulstar.Models.Products;
    using Pulstar.Web.Infrastructure;

    public abstract class BaseTest : IDisposable
    {
        protected const string DefaultCategoryName = "test-category";
        private static bool _testsInitialized = false;

        static BaseTest()
        {
            Mapper.Initialize(config => config.AddProfile<AutoMapperProfile>());
        }

        protected PulstarDbContext Db { get; set; }

        public void Dispose()
        {
            Db?.Dispose();
        }

        protected PulstarDbContext GetDatabase()
        {
            var dbOptions = new DbContextOptionsBuilder<PulstarDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new PulstarDbContext(dbOptions);
        }

        protected async Task<ProductModel> GenerateValidProduct()
        {
            var product = new Fixture().Create<ProductModel>();
            var category = new Category { Name = DefaultCategoryName };
            Db.Categories.Add(category);
            await Db.SaveChangesAsync();
            product.CategoryId = category.Id;
            return product;
        }
    }
}
