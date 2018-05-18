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
    using Pulstar.Data.Extensions;
    using Pulstar.Data.Interfaces;
    using Pulstar.DataModels;
    using Pulstar.Models.Purchase;
    using Pulstar.Services.Interfaces;

    public class PurchaseService : IPurchaseService
    {
        private readonly IRepository<Product> _productsRepostory;
        private readonly IRepository<Purchase> _purchasesRepository;

        private readonly UserManager<User> _userManager;
        private readonly IUserAccountService _userAccountService;

        public PurchaseService(
            IRepository<Product> productsRepostory,
            IRepository<Purchase> purchasesRepostory,
            UserManager<User> userManager,
            IUserAccountService userAccountService)
        {
            _productsRepostory = productsRepostory;
            _purchasesRepository = purchasesRepostory;
            _userManager = userManager;
            _userAccountService = userAccountService;
        }

        public async Task AddPurchase(IEnumerable<PurchaseProduct> purchaseProducts, string userName, string deliveryAddress)
        {
            if (string.IsNullOrEmpty(deliveryAddress))
            {
                throw new InvalidOperationException(ServiceErrorsConstants.AddressRequired);
            }

            var user = await _userManager.FindByNameAsync(userName);
            var productIds = purchaseProducts.Select(p => p.Id).ToList();
            var productsToBuy = await _productsRepostory.All()
                .Where(p => productIds.Contains(p.Id))
                .ToListAsync();

            var purchaseAmount = 0m;

            if (productsToBuy.Count != productIds.Count)
            {
                throw new InvalidOperationException(ServiceErrorsConstants.InvalidOrMissingProduct);
            }

            // TODO extract to extension method
            foreach (var dbproduct in productsToBuy)
            {
                var dbProductPrice = dbproduct.Price - (dbproduct.Price * ((decimal)dbproduct.Discount / 100));
                var productFromCartPrice = purchaseProducts.Where(p => p.Id == dbproduct.Id).Select(p => p.PriceAfterDiscount).FirstOrDefault();
                if (productFromCartPrice != dbProductPrice)
                {
                    throw new InvalidOperationException(ServiceErrorsConstants.ProductCartPriceMismatch);
                }

                purchaseAmount += productFromCartPrice;
            }

            var purchase = new Purchase
            {
                Amount = purchaseAmount,
                Products = productsToBuy,
                UserId = user.Id,
                User = user,
                DeliveryAddress = deliveryAddress,
                Date = DateTime.UtcNow,
            };

            await _purchasesRepository.AddAsync(purchase);
            
            if (!_userAccountService.HasEnoughFunds(user, purchaseAmount))
            {
                throw new InvalidOperationException(string.Format(ServiceErrorsConstants.UserInsufficientFunds, userName));
            }

            await _userAccountService.Withdraw(user, purchaseAmount);

            purchase.SetUniquePurchaseCode();
            await _purchasesRepository.UpdateAsync(purchase);
        }

        public async Task<IEnumerable<PurchaseListingModel>> Products(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new InvalidOperationException(ServiceErrorsConstants.UserNotFound);
            }

            var products = await _purchasesRepository
                .All()
                .Where(u => u.User.UserName == userName)
                .ProjectTo<PurchaseListingModel>()
                .ToListAsync();

            return products;
        }
    }
}
