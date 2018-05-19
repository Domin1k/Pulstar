namespace Pulstar.Services.Models.Products
{
    public class ProductModel : ProductListingModel
    {
        public int Quantity { get; set; }
        
        public string Manufacturer { get; set; }
        
        public int CategoryId { get; set; }

        public string Description { get; set; }
    }
}
