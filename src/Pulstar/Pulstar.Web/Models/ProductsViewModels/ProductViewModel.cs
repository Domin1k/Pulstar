namespace Pulstar.Web.Models.ProductsViewModels
{
    using System.ComponentModel.DataAnnotations;
    using Pulstar.Common.Interfaces;
    using Pulstar.Models.Products;

    public class ProductViewModel : IMapFrom<ProductDetailsModel>
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        public string Title { get; set; }

        [Required]
        [MinLength(2)]
        public string Description { get; set; }

        [Required]
        [MinLength(2)]
        public string Model { get; set; }

        [Required]
        [MinLength(2)]
        public string Manufacturer { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}
