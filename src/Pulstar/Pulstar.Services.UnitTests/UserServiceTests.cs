namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Pulstar.Common.Helpers;
    using Pulstar.Data.Models;
    using Pulstar.TestUtils;
    using Xunit;

    public class UserServiceTests : BaseTest
    {
        private UserService _userService;

        public UserServiceTests()
        {
            Db = GetDatabase();
            _userService = new UserService(Db, UserManagerMock.New.Object, null);
        }

        [Fact]
        public async Task AddPaymentMethod_WithValidData_ShouldAddPaymentMethodSuccessfully()
        {
            const string CCNumber = "4485504638364536";
            const string Cvv = "123";
            const string CardHolderName = "Card Holder Name";
            var user = new User
            {
                UserName = "test-user-5",
            };

            Db.Users.Add(user);
            await Db.SaveChangesAsync();

            await _userService.AddPaymentMethod(user.UserName, CCNumber, Cvv, CardHolderName, DateTime.UtcNow.AddMonths(1));

            var userAfterAddPaymentMethod = Db.Users.First(u => u.UserName == user.UserName);

            Assert.Single(userAfterAddPaymentMethod.CreditCards);
            Assert.Equal(CreditCardHelper.Encrypt(CCNumber), userAfterAddPaymentMethod.CreditCards.First().CreditCardNumber);
            Assert.Equal(Cvv, userAfterAddPaymentMethod.CreditCards.First().CVV);
        }
    }
}
