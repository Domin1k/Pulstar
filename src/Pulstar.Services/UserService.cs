namespace Pulstar.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper.QueryableExtensions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Constants;
    using Pulstar.Common.Extensions;
    using Pulstar.Common.Helpers;
    using Pulstar.Data.Interfaces;
    using Pulstar.DataModels;
    using Pulstar.Services.Interfaces;
    using Pulstar.Services.Models.Users;

    public class UserService : IUserService
    {
        private readonly IRepository<User> _usersRepository;
        private readonly IRepository<CreditCard> _creditCardRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            IRepository<User> usersRepostory,
            IRepository<CreditCard> creditCardRepostory,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _usersRepository = usersRepostory;
            _creditCardRepository = creditCardRepostory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task AddPaymentMethod(string userName, string creditCardNumber, string cvv, string cardHolder, DateTime expirationDate)
        {
            creditCardNumber.ThrowIfNull();
            cvv.ThrowIfNull();
            cardHolder.ThrowIfNull();
            if (expirationDate < DateTime.UtcNow)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.InvalidUserInput, nameof(creditCardNumber), nameof(cvv), nameof(expirationDate)));
            }

            var creditCardType = CreditCardHelper.GetCreditCardType(creditCardNumber);

            var user = await RetrieveUser(userName);

            if (user.CreditCards.Any(cc => cc.CreditCardNumber == creditCardNumber && cc.CVV == cvv && cc.ExpirationDate == expirationDate))
            {
                throw new InvalidOperationException(ServiceErrorsConstants.InvalidUserCC);
            }

            var creditCard = new CreditCard
            {
                CardType = creditCardType,
                CreditCardNumber = CreditCardHelper.Encrypt(creditCardNumber),
                CVV = cvv,
                IsActive = true,
                ExpirationDate = expirationDate,
                CardHolderName = cardHolder,
                OwnerId = user.Id,
            };
            user.CreditCards.Add(creditCard);
            _usersRepository.Update(user);
            await _usersRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserListingModel>> All(int takeCount = AppConstants.DefaultTakeCount)
        {
            return await _usersRepository
                .All()
                .OrderBy(u => u.UserName)
                .Take(takeCount)
                .ProjectTo<UserListingModel>()
                .ToListAsync();
        }

        public async Task AssignToRole(string userName, string roleName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var role = await _roleManager.FindByNameAsync(roleName);

            if (user == null || role == null)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.UserNameOrRoleNotExists, userName, role));
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.AddToRoleFailed, roleName, userName, string.Join(",", result.Errors.Select(e => e.Description))));
            }
        }

        public async Task DepositFunds(string userName, decimal amountToDeposit, IUserAccountService userAccountService)
        {
            if (amountToDeposit <= 0)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.InvalidDeposit, nameof(amountToDeposit)));
            }

            var user = await _usersRepository.All().Where(u => u.UserName == userName).Include(u => u.CreditCards).FirstOrDefaultAsync();

            if (!user.CreditCards.Any(c => c.IsActive))
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.UserDoesNotHaveAnyPaymentMethods, user.UserName));
            }

            await userAccountService.DepositAsync(user, amountToDeposit);
            _usersRepository.Update(user);
            await _usersRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserPaymentMethodsModel>> PaymentMethods(string userName)
        {
            var user = await RetrieveUser(userName);

            return await _creditCardRepository
                .All()
                .Where(c => c.OwnerId == user.Id && c.IsActive)
                .OrderByDescending(cc => cc.ExpirationDate)
                .ProjectTo<UserPaymentMethodsModel>()
                .ToListAsync();
        }

        private async Task<User> RetrieveUser(string userName)
        {
            var user = await _usersRepository.All().FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.UserNameNotFound, userName));
            }

            return user;
        }
    }
}
