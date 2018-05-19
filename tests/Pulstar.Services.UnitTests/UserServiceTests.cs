namespace Pulstar.Services.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Pulstar.Common.Helpers;
    using Pulstar.Data.Repositories;
    using Pulstar.DataModels;
    using Pulstar.TestUtils;
    using Xunit;

    public class UserServiceTests : BaseTest
    {
        private UserService _userService;
        private GenericRepository<CreditCard> _creditCardRepo;
        private GenericRepository<User> _userRepo;

        public UserServiceTests()
        {
            var db = GetDatabase();
            _creditCardRepo = new GenericRepository<CreditCard>(db);
            _userRepo = new GenericRepository<User>(db);
            _userService = new UserService(_userRepo, _creditCardRepo, UserManagerMock.New.Object, null);
        }

        /* Needs work
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

            _userRepo.Add(user);
            await _userRepo.SaveChangesAsync();
            await _userService.AddPaymentMethod(user.UserName, CCNumber, Cvv, CardHolderName, DateTime.UtcNow.AddMonths(1));

            var userAfterAddPaymentMethod = _userRepo.All().First(u => u.UserName == user.UserName);

            Assert.Single(userAfterAddPaymentMethod.CreditCards);
            Assert.Equal(CreditCardHelper.Encrypt(CCNumber), userAfterAddPaymentMethod.CreditCards.First().CreditCardNumber);
            Assert.Equal(Cvv, userAfterAddPaymentMethod.CreditCards.First().CVV);
        }*/
    }
}
