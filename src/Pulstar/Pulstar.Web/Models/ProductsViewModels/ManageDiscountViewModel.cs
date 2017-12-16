namespace Pulstar.Web.Models.ProductsViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ManageDiscountViewModel
    {
        [Range(0, int.MaxValue)]
        public int ProductId { get; set; }

        [Range(1, 99)]
        public double Discount { get; set; }
    }
}
