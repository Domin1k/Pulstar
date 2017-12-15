namespace Pulstar.Models.Products
{
    using AutoMapper;
    using Pulstar.Common.Interfaces;
    using Pulstar.Data.Models;

    public class ProductDetailsModel : ProductListingModel, IHaveCustomMapping
    {
        public string Description { get; set; }

        public string Manufacturer { get; set; }

        public int Quantity { get; set; }

        public string Category { get; set; }
        
        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<Product, ProductDetailsModel>()
                .ForMember(pm => pm.Category, opt => opt.MapFrom(p => p.Category.Name));
        }
    }
}
