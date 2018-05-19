namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using Pulstar.Data.Repositories;
    using Pulstar.DataModels;
    using Pulstar.Services.Interfaces;
    using Pulstar.Services.Models.Purchase;
    using Pulstar.TestUtils;
    using Xunit;

    public class PurchaseServiceTests : BaseTest
    {
        private const string UserName = "test-user";
        private const string TestAddress = "Bulgaria;Sofia;1000";
        private const decimal RichUserAccount = 15000;
        private string _userId = Guid.NewGuid().ToString();
        private PurchaseService _purchaseService;
        private GenericRepository<Product> _productRepo;
        private GenericRepository<Category> _categoryRepo;
        private GenericRepository<Purchase> _purchaseRepo;

        public PurchaseServiceTests()
        {
            var user = new User() { UserName = UserName, Id = _userId, AccountBalance = RichUserAccount };
            var db = GetDatabase();
            var userManager = UserManagerMock.New;
            userManager.Setup(s => s.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            var userAccountMock = new Mock<IUserAccountService>();
            userAccountMock.Setup(u => u.WithdrawAsync(It.IsAny<User>(), It.IsAny<decimal>()));
            userAccountMock.Setup(u => u.HasEnoughFunds(It.IsAny<User>(), It.IsAny<decimal>())).Returns(true);
            _productRepo = new GenericRepository<Product>(db);
            _categoryRepo = new GenericRepository<Category>(db);
            _purchaseRepo = new GenericRepository<Purchase>(db);
            _purchaseService = new PurchaseService(_productRepo, _purchaseRepo, userManager.Object, userAccountMock.Object);
        }

        /* Needs work
        [Fact]
        public async Task AddPurchase_WithValidData_ShouldPersistPurchase()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            var dbProduct = new Product
            {
                CategoryId = product.CategoryId,
                Title = product.Title,
                Model = product.Model,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                Discount = product.Discount,
                Quantity = product.Quantity,
            };
            _productRepo.Add(dbProduct);

            var productToPurchase = new List<PurchaseProduct>
            {
                new PurchaseProduct
                {
                    Id = dbProduct.Id,
                    PriceAfterDiscount = dbProduct.Price - (dbProduct.Price * (decimal)(dbProduct.Discount / 100)),
                },
            };

            await _purchaseService.AddPurchase(productToPurchase, UserName, TestAddress);

            Assert.True(_purchaseRepo.All().Any(p => p.User.Id == _userId));
        }*/

        [Fact]
        public async Task AddPurchase_WithPriceMismatch_ShouldThrow()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            var dbProduct = new Product
            {
                CategoryId = product.CategoryId,
                Title = product.Title,
                Model = product.Model,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                Discount = product.Discount,
                Quantity = product.Quantity,
            };
            _productRepo.Add(dbProduct);

            var productToPurchase = new List<PurchaseProduct>
            {
                new PurchaseProduct
                {
                    Id = dbProduct.Id,
                    PriceAfterDiscount = (decimal)new Random().Next(23, 2323),
                },
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _purchaseService.AddPurchase(productToPurchase, UserName, TestAddress));
        }

        [Fact]
        public async Task AddPurchase_WithInvalidProducts_ShouldThrow()
        {
            var product = await GenerateValidProduct(_categoryRepo);
            var dbProduct = new Product
            {
                CategoryId = product.CategoryId,
                Title = product.Title,
                Model = product.Model,
                Manufacturer = product.Manufacturer,
                Price = product.Price,
                Discount = product.Discount,
                Quantity = product.Quantity,
            };
            _productRepo.Add(dbProduct);

            var productToPurchase = new List<PurchaseProduct>
            {
                new PurchaseProduct
                {
                    Id = new Random().Next(233, 500),
                    PriceAfterDiscount = dbProduct.Price - (dbProduct.Price * (decimal)(dbProduct.Discount / 100)),
                },
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _purchaseService.AddPurchase(productToPurchase, UserName, TestAddress));
        }
    }
}
