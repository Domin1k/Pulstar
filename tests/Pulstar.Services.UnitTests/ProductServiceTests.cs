namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Ploeh.AutoFixture;
    using Pulstar.Data.Interfaces;
    using Pulstar.Data.Repositories;
    using Pulstar.DataModels;
    using Pulstar.Services.Models.Products;
    using Pulstar.TestUtils;
    using Xunit;

    public class ProductServiceTests : BaseTest
    {
        private ProductService _productService;
        private GenericRepository<Product> _productRepo;
        private GenericRepository<Category> _categoryRepo;

        public ProductServiceTests()
        {
            _productRepo = new GenericRepository<Product>(GetDatabase());
            _categoryRepo = new GenericRepository<Category>(GetDatabase());
            _productService = new ProductService(_productRepo, _categoryRepo, UserManagerMock.New.Object);
        }

        [Fact]
        public async Task AddProduct_WithValidData_ShouldAddProductInDb()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            await _productService.AddProduct(product);

            Assert.Equal(1, _productRepo.All().Count());
        }

        [Fact]
        public async Task AddProduct_WithNullData_ShouldThrow()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            product.Title = null;

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _productService.AddProduct(product));
        }

        [Fact]
        public async Task AddProduct_WithNonExistingCategory_ShouldThrow()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            product.CategoryId = product.CategoryId + 42;

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _productService.AddProduct(product));
        }

        /* Needs work
        [Fact]
        public async Task All_WithExistingCategory_ShouldReturnOnlyGiveCategoryProducts()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            var category = _categoryRepo.All().FirstOrDefault(c => c.Name == DefaultCategoryName);
            _productRepo.Add(new Product { CategoryId = category.Id });
            var products = await _productService.All(category.Name, p => p.Id, Common.Enums.OrderType.Descending);

            Assert.NotEmpty(products);
            Assert.True(products.All(p => p.CategoryId == category.Id));
        }*/

        [Fact]
        public async Task All_WithNonExistingCategory_ShouldThrow()
        {
            var product = await GenerateValidProduct(_categoryRepo);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _productService.All("non-existing-category", p => p.Id, Common.Enums.OrderType.Descending));
        }
        
        /* Needs work
        public async Task EditProduct_WithValidData_ShouldEditProductCorrectly()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            var editProduct = new ProductModel();
            editProduct.Price = 10;
            editProduct.Quantity = 10;
            editProduct.Title = "Title";
            editProduct.Model = "EditedModel";
            editProduct.Manufacturer = "EditedManufacturer";
            editProduct.Discount = 0;
            editProduct.Description = "EditedDescription";
            editProduct.Image = new byte[100];

            product.Price = product.Price;
            product.Quantity = product.Quantity;
            product.Title = product.Title;
            product.Model = product.Model;
            product.Manufacturer = product.Manufacturer;
            product.Discount = product.Discount;
            product.Description = product.Description;
            product.Image = new byte[1024];

            Assert.NotEqual(product.Price, editProduct.Price);
            Assert.NotEqual(product.Quantity, editProduct.Quantity);
            Assert.NotEqual(product.Title, editProduct.Title);
            Assert.NotEqual(product.Model, editProduct.Model);
            Assert.NotEqual(product.Manufacturer, editProduct.Manufacturer);
            Assert.NotEqual(product.Discount, editProduct.Discount);
            Assert.NotEqual(product.Description, editProduct.Description);
            Assert.NotEqual(product.Image, editProduct.Image);
        }*/
    }
}
