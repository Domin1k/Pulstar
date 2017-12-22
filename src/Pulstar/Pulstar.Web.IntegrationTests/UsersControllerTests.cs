namespace Pulstar.Web.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using Moq;
    using Ploeh.AutoFixture;
    using Pulstar.Models.Products;
    using Pulstar.Models.Users;
    using Pulstar.Services.Interfaces;
    using Pulstar.TestUtils;
    using Pulstar.Web.Controllers;
    using Pulstar.Web.Models.UsersViewModels;
    using Xunit;

    public class UsersControllerTests
    {
        [Fact]
        public void AddToCart_WithValidObject_ShouldAddObjectToCart()
        {
            const int objectIdToAdd = 12;
            const string currentUserName = "test-user";

            var fakeSession = new Dictionary<string, object>();
            var controller = InstanciateWithMocks();
            PrepareController(controller, currentUserName, fakeSession);
            controller.AddToCart(objectIdToAdd);

            Assert.True(fakeSession.Count > 0);
        }

        [Fact]
        public void Cart_WithTwoUsers_ShouldNotMixTheirCartItems()
        {
            const int objectIdToAdd1 = 12;
            const int objectIdToAdd2 = 12;
            const string currentUserName1 = "test-user";
            const string currentUserName2 = "another-test-user";

            var firstUserSession = new Dictionary<string, object>();
            var secondUserSession = new Dictionary<string, object>();

            var firstController = InstanciateWithMocks();
            PrepareController(firstController, currentUserName1, firstUserSession);
            firstController.AddToCart(objectIdToAdd1);

            var secondController = InstanciateWithMocks();
            PrepareController(secondController, currentUserName2, secondUserSession);
            secondController.AddToCart(objectIdToAdd2);

            Assert.Single(firstUserSession.Keys);
            Assert.Single(secondUserSession.Keys);
        }

        [Fact]
        public async Task Checkout_WithCreditCardIdButWithoutExistingCreditCards_ShouldNotThrow()
        {
            const string currentUserName = "test-user";

            var fakeSession = new Dictionary<string, object>();
            var controller = InstanciateWithMocks();
            PrepareController(controller, currentUserName, fakeSession);
            var products = new Fixture().CreateMany<ProductModel>().ToList();
            var model = new UserCheckoutCartViewModel
            {
                CartProducts = products,
                CreditCardId = "4485504638364536",
                TotalCost = products.Sum(p => p.Price),
                Address = "Test adress",
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await controller.Checkout(model));
        }

        [Fact]
        public void Deposit_WithValidInput_ShouldIncrementUserAccountBalance()
        {
            const string currentUserName = "test-user";

            var fakeSession = new Dictionary<string, object>();
            var controller = InstanciateWithMocks();
            PrepareController(controller, currentUserName, fakeSession);
            var products = new Fixture().CreateMany<ProductModel>().ToList();
            var model = new UserCheckoutCartViewModel
            {
                CartProducts = products,
                CreditCardId = "4485504638364536",
                TotalCost = products.Sum(p => p.Price),
                Address = "Test adress",
            };
        }

        private UsersController InstanciateWithMocks()
        {
            var productServiceMock = new Mock<IProductService>().Object;
            var userAccountServiceMock = new Mock<IUserAccountService>().Object;
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(u => u.PaymentMethods(It.IsAny<string>())).ReturnsAsync(Enumerable.Empty<UserPaymentMethodsModel>());
            var purchaseServiceMock = new Mock<IPurchaseService>().Object;
            var tempDataMock = new Mock<ITempDataDictionary>();
            var controller = new UsersController(productServiceMock, userServiceMock.Object, userAccountServiceMock, purchaseServiceMock, UserManagerMock.New.Object);
            controller.TempData = tempDataMock.Object;
            return controller;
        }
        
        private void PrepareController(UsersController userController, string userName, Dictionary<string, object> fakeSession)
        {
            var mockSession = GetSession();
            mockSession
                .Setup(s => s.Set(It.IsAny<string>(), It.IsAny<byte[]>()))
                .Callback((string key, object value) => fakeSession[key] = value);
            var mockHttpContext1 = new Mock<HttpContext>();
            mockHttpContext1.Setup(s => s.Session).Returns(mockSession.Object);
            mockHttpContext1.Setup(s => s.User).Returns(new ClaimsPrincipal(new GenericPrincipal(new GenericIdentity(userName), new string[0])));
            userController.ControllerContext.HttpContext = mockHttpContext1.Object;
        }

        private Mock<ISession> GetSession()
        {
            var mockSession = new Mock<ISession>();
            var moqSetupParam = new byte[0];
            mockSession.Setup(s => s.TryGetValue(It.IsAny<string>(), out moqSetupParam)).Returns(false);
            return mockSession;
        }
    }
}
