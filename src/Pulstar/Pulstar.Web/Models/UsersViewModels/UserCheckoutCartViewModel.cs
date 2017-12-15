namespace Pulstar.Web.Models.UsersViewModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Pulstar.Models.Products;

    public class UserCheckoutCartViewModel
    {
        public List<ProductModel> CartProducts { get; set; } = new List<ProductModel>();

        public decimal TotalCost { get; set; }

        [Required]
        public string Address { get; set; }

        [Required(ErrorMessage = "Credit card is mandatory!")]
        public string CreditCardId { get; set; }

        public List<SelectListItem> CreditCards { get; set; } = new List<SelectListItem>();
    }
}
