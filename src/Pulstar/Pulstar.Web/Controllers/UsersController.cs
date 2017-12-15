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
    using Pulstar.Common.Helpers;
    using Pulstar.Data.Models;
    using Pulstar.Models.Products;
    using Pulstar.Services;
    using Pulstar.Services.Interfaces;
    using Pulstar.Web.Models.UsersViewModels;

    [Authorize]
    public class UsersController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UsersController(IProductService productService, IUserService userService, UserManager<User> userManager)
        {
            _productService = productService;
            _userService = userService;
            _userManager = userManager;
        }

        protected string Key => $"{User.Identity.Name}-Cart";

        public async Task<IActionResult> Cart()
        {
            ViewBag.CartTitle = "Cart";
            var model = await GetCurrentUserCartItems();
            return View(model);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            if (id <= 0)
            {
                TempData.AddErrorMessage("Invalid product.");

                // TODO
                return Ok();
            }

            var productCategory = await _productService.GetCategory(id);
            var cartItems = new List<int>() { id };
            var currentCart = HttpContext.Session.Get<List<int>>(Key);
            if (currentCart != null && currentCart.Any())
            {
                cartItems.AddRange(currentCart);
            }

            HttpContext.Session.Set<List<int>>(Key, cartItems);
            TempData.AddSuccessMessage("Added to cart successfully");
            var redirectUrl = $"/Products/{UrlHelper.GenerateUrl(productCategory.categoryType)}/{productCategory.category}";
            return Redirect(redirectUrl);
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
                var userName = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userName);
                var userAccount = new UserAccountService(user);
                await _userService.BuyProducts(userName, cartItems, userAccount);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                throw;
            }

            TempData.AddSuccessMessage("Thank you for your purchase!");
            return Redirect("/");
        }

        [HttpGet]
        public IActionResult AddPaymentMethod()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Deposit()
        {
            return View();
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
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                var userAccount = new UserAccountService(user);
                await _userService.DepositFunds(User.Identity.Name, model.AmountToDeposit, userAccount);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                throw;
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
                throw;
            }

            return RedirectToAction(nameof(ManageController.Index), "Manage");
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
