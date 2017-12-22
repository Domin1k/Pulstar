namespace Pulstar.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Pulstar.Data.Models;
    using Pulstar.Models.Products;
    using Pulstar.Models.Purchase;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Infrastructure.Constants;
    using Pulstar.Web.Models.UsersViewModels;

    [Authorize]
    public class UsersController : Controller
    {
        private const string ProductDetails = "/products/details/{0}";
        private const string Home = "/";
        private const string ManageControllerName = "Manage";
        private const string UsersControllerName = "Users";
        
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IUserAccountService _userAccountService;
        private readonly IPurchaseService _purchaseService;
        private readonly UserManager<User> _userManager;

        public UsersController(
            IProductService productService,
            IUserService userService,
            IUserAccountService userAccountService,
            IPurchaseService purchaseService,
            UserManager<User> userManager)
        {
            _productService = productService;
            _userService = userService;
            _userAccountService = userAccountService;
            _purchaseService = purchaseService;
            _userManager = userManager;
        }

        protected string Key => $"{User.Identity.Name}-Cart";

        public async Task<IActionResult> Cart()
        {
            var model = await GetCurrentUserCartItems();
            return View(model);
        }

        public IActionResult AddToCart(int id)
        {
            if (id <= 0)
            {
                TempData.AddErrorMessage(TempMessages.InvalidProduct);
                
                return Ok();
            }

            var cartItems = new List<int>() { id };
            var currentCart = HttpContext.Session.Get<List<int>>(Key);
            if (currentCart != null && currentCart.Any(i => i == id))
            {
                TempData.AddErrorMessage(TempMessages.AlreadyInCart);
                return Redirect(string.Format(ProductDetails, id));
            }

            if (currentCart != null && currentCart.Any())
            {
                cartItems.AddRange(currentCart);
            }

            HttpContext.Session.Set<List<int>>(Key, cartItems);
            TempData.AddSuccessMessage(TempMessages.AddedSuccefully);
            return Redirect(string.Format(ProductDetails, id));
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var creditCards = (await _userService.PaymentMethods(User.Identity.Name))
                .Select(u => new SelectListItem
                {
                    Value = u.CreditCardNumber,
                    Text = u.CreditCardNumber,
                })
                .ToList();
            var cartProducts = await GetCurrentUserCartItems();
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var model = new UserCheckoutCartViewModel
            {
                CartProducts = cartProducts,
                CreditCards = creditCards,
                TotalCost = cartProducts.Sum(p => p.Price),
                PhoneNumber = user.PhoneNumber,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(UserCheckoutCartViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var cartItems = await GetCurrentUserCartItems();
                var purchaseProduct = cartItems
                    .Select(c => new PurchaseProduct
                    {
                        Id = c.Id,
                        PriceAfterDiscount = c.PriceAfterDiscount,
                    })
                    .ToList();
                await _purchaseService.AddPurchase(purchaseProduct, User.Identity.Name, model.Address);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return RedirectToAction(nameof(UsersController.Checkout), UsersControllerName);
            }

            TempData.AddSuccessMessage(TempMessages.ThankYouPurchase);
            return Redirect(Home);
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositFundsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return RedirectToAction(nameof(ManageController.Deposit), ManageControllerName);
            }

            try
            {
                await _userService.DepositFunds(User.Identity.Name, model.AmountToDeposit, _userAccountService);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return RedirectToAction(nameof(ManageController.Deposit), ManageControllerName);
            }

            TempData.AddSuccessMessage(TempMessages.SuccessDeposit);
            return RedirectToAction(nameof(ManageController.Index), ManageControllerName);
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentMethod(AddPaymentMethodViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(string.Join(Environment.NewLine, ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))));
                return RedirectToAction(nameof(ManageController.AddPaymentMethod), ManageControllerName);
            }

            try
            {
                await _userService.AddPaymentMethod(User.Identity.Name, model.CreditCardNumber, model.CVV, model.CardHolderName, model.ExpirationDate);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return RedirectToAction(nameof(ManageController.Index), ManageControllerName);
            }

            TempData.AddSuccessMessage(TempMessages.AddedPaymentMethodSuccess);
            return RedirectToAction(nameof(ManageController.Index), ManageControllerName);
        }

        [HttpGet]
        public async Task<IActionResult> MyProducts()
        {
            IEnumerable<PurchaseListingModel> products = null;
            try
            {
                products = await _purchaseService.Products(User.Identity.Name);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return Redirect(Home);
            }

            return View(products ?? Enumerable.Empty<PurchaseListingModel>());
        }

        private async Task<List<ProductModel>> GetCurrentUserCartItems()
        {
            var cart = HttpContext.Session.Get<List<int>>(Key);
            var model = cart != null
                ? (await _productService.List(cart)).ToList()
                : new List<ProductModel>();

            return model;
        }
    }
}
