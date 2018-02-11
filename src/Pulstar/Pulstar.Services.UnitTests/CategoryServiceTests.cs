namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Pulstar.Common.Enums;
    using Xunit;

    public class CategoryServiceTests : BaseTest
    {
        private CategoryService _categoryService;

        public CategoryServiceTests()
        {
            Db = GetDatabase();
            _categoryService = new CategoryService(Db);
        }

        [Fact]
        public async Task AddCategory_WithCorrectData_ShouldAddCategory()
        {
            const string catName = "test-category";
            var currentCategoriesBefore = Db.Categories.Count();
            Assert.Equal(0, currentCategoriesBefore);

            await _categoryService.AddCategory(catName, CategoryType.Game);

            var currentCategoriesAfter = Db.Categories.Count();
            Assert.Equal(1, currentCategoriesAfter);
        }

        [Fact]
        public async Task AddCategory_WithExistingCategory_ShouldNotAddCategoryAndThrow()
        {
            const string catName = "test-category";

            await _categoryService.AddCategory(catName, CategoryType.Game);
            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _categoryService.AddCategory(catName, CategoryType.Accessory));

            var currentCategoriesAfter = Db.Categories.Count();
            Assert.Equal(1, currentCategoriesAfter);
        }

        [Fact]
        public async Task DeleteCategory_WithExistingCategory_ShouldDeleteCategoryCorrectly()
        {
            const string catName = "test-category";

            await _categoryService.AddCategory(catName, CategoryType.Game);
            await _categoryService.DeleteCategory(catName);
            var category = Db.Categories.First(c => c.Name == catName);
            Assert.True(category.IsDeleted);
        }
    }
}
