namespace Pulstar.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Pulstar.Models.Products;
    using Pulstar.Models.Purchase;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Models.UsersViewModels;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly IUserAccountService _userAccountService;
        private readonly IPurchaseService _purchaseService;

        public UsersController(
            IProductService productService,
            IUserService userService,
            IUserAccountService userAccountService,
            IPurchaseService purchaseService)
        {
            _productService = productService;
            _userService = userService;
            _userAccountService = userAccountService;
            _purchaseService = purchaseService;
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
                TempData.AddErrorMessage("Invalid product.");

                // TODO
                return Ok();
            }

            var cartItems = new List<int>() { id };
            var currentCart = HttpContext.Session.Get<List<int>>(Key);
            if (currentCart != null && currentCart.Any())
            {
                cartItems.AddRange(currentCart);
            }

            HttpContext.Session.Set<List<int>>(Key, cartItems);
            TempData.AddSuccessMessage("Added to cart successfully");
            return Redirect($"/products/details/{id}");
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
            var model = new UserCheckoutCartViewModel
            {
                CartProducts = cartProducts,
                CreditCards = creditCards,
                TotalCost = cartProducts.Sum(p => p.Price),
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
                await _purchaseService.AddPurchase(purchaseProduct, User.Identity.Name);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return RedirectToAction(nameof(UsersController.Checkout), "Users");
            }

            TempData.AddSuccessMessage("Thank you for your purchase!");
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Deposit(DepositFundsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _userService.DepositFunds(User.Identity.Name, model.AmountToDeposit, _userAccountService);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return RedirectToAction(nameof(UsersController.Deposit), "Users");
            }

            TempData.AddSuccessMessage("Successfully deposited funds.");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentMethod(AddPaymentMethodViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _userService.AddPaymentMethod(User.Identity.Name, model.CreditCardNumber, model.CVV, model.ExpirationDate);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return RedirectToAction(nameof(UsersController.AddPaymentMethod), "Users");
            }

            TempData.AddSuccessMessage("You have succesfully added payment method to user.");
            return RedirectToAction(nameof(ManageController.Index), "Manage");
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
                return Redirect("/");
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
