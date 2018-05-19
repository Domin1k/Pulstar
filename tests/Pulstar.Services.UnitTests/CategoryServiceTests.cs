namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Pulstar.Common.Enums;
    using Pulstar.Data.Repositories;
    using Pulstar.DataModels;
    using Xunit;

    public class CategoryServiceTests : BaseTest
    {
        private GenericRepository<Category> _categoryRepo;
        private CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _categoryRepo = new GenericRepository<Category>(GetDatabase());
            _categoryService = new CategoryService(_categoryRepo);
        }

        [Fact]
        public async Task AddCategory_WithCorrectData_ShouldAddCategory()
        {
            const string catName = "test-category";
            var currentCategoriesBefore = _categoryRepo.All().Count();
            Assert.Equal(0, currentCategoriesBefore);

            await _categoryService.AddCategory(catName, CategoryType.Game);

            var currentCategoriesAfter = _categoryRepo.All().Count();
            Assert.Equal(1, currentCategoriesAfter);
        }

        [Fact]
        public async Task AddCategory_WithExistingCategory_ShouldNotAddCategoryAndThrow()
        {
            const string catName = "test-category";

            await _categoryService.AddCategory(catName, CategoryType.Game);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _categoryService.AddCategory(catName, CategoryType.Accessory));

            var currentCategoriesAfter = _categoryRepo.All().Count();
            Assert.Equal(1, currentCategoriesAfter);
        }

        [Fact]
        public async Task DeleteCategory_WithExistingCategory_ShouldDeleteCategoryCorrectly()
        {
            const string catName = "test-category";
            
            await _categoryService.AddCategory(catName, CategoryType.Game);
            await _categoryService.DeleteCategory(catName);
            var category = _categoryRepo.AllWithDeleted().FirstOrDefault(c => c.Name == catName);
            Assert.True(category.IsDeleted);
        }
    }
}
