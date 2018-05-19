namespace Pulstar.Web.Areas.Admin.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Pulstar.Common.Constants;
    using Pulstar.Common.Enums;
    using Pulstar.Services.Interfaces;
    using Pulstar.Services.Models.Products;
    using Pulstar.Web.Areas.Admin.Models.Categories;
    using Pulstar.Web.Areas.Admin.Models.Users;
    using Pulstar.Web.Extensions;
    using Pulstar.Web.Infrastructure.Constants;
    using Pulstar.Web.Infrastructure.Extensions;
    using Pulstar.Web.Models.ProductsViewModels;

    [Area(WebContants.AdminArea)]
    [Authorize(Roles = AppConstants.Administrator + "," + AppConstants.Manager)]
    public class AdminsController : Controller
    {
        private const string ProductDetails = "/products/details/{0}";
        private const string AdminController = "Admins";
        private const string Home = "/";
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public AdminsController(
            RoleManager<IdentityRole> roleManager,
            IUserService userService,
            IProductService productService,
            ICategoryService categoryService)
        {
            _roleManager = roleManager;
            _userService = userService;
            _productService = productService;
            _categoryService = categoryService;
        }

        [Authorize(Roles = AppConstants.Administrator)]
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(r => new SelectListItem { Text = r.Name, Value = r.Name }).ToListAsync();
            var users = await _userService.All();
            var model = users
                .Select(u => new ManageUserViewModel
                {
                    UserName = u.UserName,
                    Email = u.Email,
                    Roles = roles,
                })
                .ToList();
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = AppConstants.Administrator)]
        public IActionResult ManageCategory(string id) 
            => View("ManageCategory", id);

        [Authorize(Roles = AppConstants.Administrator)]
        [HttpGet]
        public IActionResult AddCategory()
        {
            var categoryTypes = Enum
                .GetValues(typeof(CategoryType))
                .Cast<CategoryType>()
                .Select(r => new SelectListItem { Text = r.ToString(), Value = r.ToString() })
                .ToList();
            return View(new AddCategoryViewModel { CategoryTypes = categoryTypes });
        }

        [Authorize(Roles = AppConstants.Administrator)]
        [HttpPost]
        public async Task<IActionResult> AddCategory(AddCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
                return RedirectToAction(nameof(AdminsController.AddCategory));
            }

            CategoryType catType;

            if (!Enum.TryParse<CategoryType>(model.CategoryTypeId, out catType))
            {
                TempData.AddErrorMessage(string.Format(TempMessages.InvalidCategoryType, model.Name));
                return RedirectToAction(nameof(AdminsController.AddCategory));
            }

            try
            {
                await _categoryService.AddCategory(model.Name, catType);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }

            TempData.AddSuccessMessage(string.Format(TempMessages.AddedCategorySuccess, model.Name));

            return Redirect(Home);
        }

        [Authorize(Roles = AppConstants.Administrator)]
        public IActionResult DeleteCategory(string id) => View(new ManageCategoryViewModel { Name = id });

        [HttpPost]
        [Authorize(Roles = AppConstants.Administrator)]
        public async Task<IActionResult> DeleteCategory(ManageCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
                return RedirectToAction(nameof(AdminsController.DeleteCategory), new { id = model.Name });
            }

            try
            {
                await _categoryService.DeleteCategory(model.Name);
            }
            catch (InvalidOperationException)
            {
                TempData.AddErrorMessage(TempMessages.ErrorDuringCategoryDelete);
            }

            TempData.AddSuccessMessage(TempMessages.SuccessCategoryDelete);

            return RedirectToAction(nameof(AdminsController.Index), AdminController);
        }

        public IActionResult EditCagory(string id)
        {
            // TODO
            return RedirectToAction(nameof(AdminsController.Index), AdminController);
        }

        [Authorize(Roles = AppConstants.Administrator)]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
                return Redirect(string.Format(ProductDetails, id));
            }

            try
            {
                await _productService.DeleteProduct(id);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
                return Redirect(string.Format(ProductDetails, id));
            }

            return Redirect(Home);
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductViewModel input, IFormFile image)
        {
            if (image.Length > AppConstants.MaximumImageSizeInBytesAllowed)
            {
                TempData.AddErrorMessage(string.Format(TempMessages.MaximumImageSizeExceeded, AppConstants.MaximumImageSizeInBytesAllowed));
                return Redirect(string.Format(ProductDetails, input.Id));
            }

            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
                return Redirect(string.Format(ProductDetails, input.Id));
            }

            var productModel = new ProductModel
            {
                Description = input.Description,
                Manufacturer = input.Manufacturer,
                Model = input.Model,
                Price = input.Price,
                Discount = input.Discount,
                Quantity = input.Quantity,
                Title = input.Title,
            };
            var byteImage = await image.ToByteArrayAsync();

            try
            {
                await _productService.EditProduct(input.Id, productModel, byteImage);
            }
            catch (InvalidOperationException ex)
            {
                TempData.AddErrorMessage(ex.Message);
            }

            return Redirect(string.Format(ProductDetails, input.Id));
        }

        [HttpPost]
        public async Task<IActionResult> ManageDiscount(ManageDiscountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
                return Redirect(string.Format(ProductDetails, model.ProductId));
            }

            try
            {
                await _productService.AddDiscount(model.ProductId, model.Discount);
            }
            catch (InvalidOperationException)
            {
                TempData.AddErrorMessage(TempMessages.InvalidDiscount);
            }

            return Redirect(string.Format(ProductDetails, model.ProductId));
        }

        [Authorize(Roles = AppConstants.Administrator)]
        [HttpPost]
        public async Task<IActionResult> Manage(AddToRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData.AddErrorMessage(TempMessages.GeneralInvalidInputData);
            }
            else
            {
                try
                {
                    await _userService.AssignToRole(model.UserName, model.Role);
                }
                catch (InvalidOperationException ex)
                {
                    TempData.AddErrorMessage(ex.Message);
                }
            }

            return RedirectToAction(nameof(AdminsController.Index), AdminController);
        }
    }
}
