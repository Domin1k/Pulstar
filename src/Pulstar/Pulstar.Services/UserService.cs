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
    using Pulstar.Data;
    using Pulstar.Data.Models;
    using Pulstar.Models.Users;
    using Pulstar.Services.Interfaces;

    public class UserService : IUserService
    {
        private readonly PulstarDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            PulstarDbContext dbContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
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
                throw new InvalidOperationException($"Invalid user input! {nameof(creditCardNumber)} must be a valid CC number, {nameof(cvv)} must be valid CVV and {nameof(expirationDate)} must NOT be past date.");
            }
            
            var creditCardType = CreditCardHelper.GetCreditCardType(creditCardNumber);

            var user = await RetrieveUser(userName);

            if (user.CreditCards.Any(cc => cc.CreditCardNumber == creditCardNumber && cc.CVV == cvv && cc.ExpirationDate == expirationDate))
            {
                throw new InvalidOperationException($"User already has this CC added.");
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
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserListingModel>> All(int takeCount = AppConstants.DefaultTakeCount)
        {
            return await _dbContext
                .Users
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
                throw new InvalidOperationException($"{userName}/{role} does not exists!");
            }

            var result = await _userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Add to {roleName} for {userName} failed.");
            }
        }
        
        public async Task DepositFunds(string userName, decimal amountToDeposit, IUserAccountService userAccountService)
        {
            if (amountToDeposit <= 0)
            {
                throw new InvalidOperationException($"{nameof(amountToDeposit)} cannot be less or equal to 0.");
            }

            var user = await _dbContext.Users.Where(u => u.UserName == userName).Include(u => u.CreditCards).FirstOrDefaultAsync();

            if (!user.CreditCards.Any(c => c.IsActive))
            {
                throw new InvalidOperationException($"{user.UserName} does not have any payment methods available. Please add payment method and try again.");
            }

            userAccountService.Deposit(user, amountToDeposit);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserPaymentMethodsModel>> PaymentMethods(string userName)
        {
            var user = await RetrieveUser(userName);

            return await _dbContext.CreditCards
                .Where(c => c.OwnerId == user.Id && c.IsActive)
                .OrderByDescending(cc => cc.ExpirationDate)
                .ProjectTo<UserPaymentMethodsModel>()
                .ToListAsync();
        }

        private async Task<User> RetrieveUser(string userName)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new InvalidOperationException($"User {userName} does not exists");
            }

            return user;
        }
    }
}
