namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Moq;
    using Pulstar.Data.Models;
    using Pulstar.Models.Purchase;
    using Pulstar.Services.Interfaces;
    using Pulstar.TestUtils;
    using Xunit;

    public class PurchaseServiceTests : BaseTest
    {
        private const string UserName = "test-user";
        private const string TestAddress = "Bulgaria;Sofia;1000";
        private const decimal RichUserAccount = 15000;
        private string _userId = Guid.NewGuid().ToString();
        private PurchaseService _purchaseService;

        public PurchaseServiceTests()
        {
            var user = new User() { UserName = UserName, Id = _userId, AccountBalance = RichUserAccount };
            Db = GetDatabase();
            var userManager = UserManagerMock.New;
            userManager.Setup(s => s.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);

            var userAccountMock = new Mock<IUserAccountService>();
            userAccountMock.Setup(u => u.Withdraw(It.IsAny<User>(), It.IsAny<decimal>()));
            userAccountMock.Setup(u => u.HasEnoughFunds(It.IsAny<User>(), It.IsAny<decimal>())).Returns(true);
            _purchaseService = new PurchaseService(Db, userManager.Object, userAccountMock.Object);
        }

        [Fact]
        public async Task AddPurchase_WithValidData_ShouldPersistPurchase()
        {
            var product = await GenerateValidProduct();
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
            Db.Products.Add(dbProduct);

            await Db.SaveChangesAsync();
            var productToPurchase = new List<PurchaseProduct>
            {
                new PurchaseProduct
                {
                    Id = dbProduct.Id,
                    PriceAfterDiscount = dbProduct.Price - (dbProduct.Price * (decimal)(dbProduct.Discount / 100)),
                },
            };

            await _purchaseService.AddPurchase(productToPurchase, UserName, TestAddress);

            Assert.True(Db.Purchases.Any(p => p.User.Id == _userId));
        }

        [Fact]
        public async Task AddPurchase_WithPriceMismatch_ShouldThrow()
        {
            var product = await GenerateValidProduct();
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
            Db.Products.Add(dbProduct);

            await Db.SaveChangesAsync();
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
            var product = await GenerateValidProduct();
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
            Db.Products.Add(dbProduct);

            await Db.SaveChangesAsync();
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
