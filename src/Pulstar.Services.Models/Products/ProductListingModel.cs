﻿namespace Pulstar.Services.Models.Products
{
    using Pulstar.Common.Interfaces;
    using Pulstar.DataModels;

    public class ProductListingModel : IMapFrom<Product>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public decimal Price { get; set; }

        public double Discount { get; set; }

        public int CategoryId { get; set; }

        public string Model { get; set; }

        public byte[] Image { get; set; }

        public decimal PriceAfterDiscount => Price - (Price * ((decimal)Discount / 100));
    }
}
