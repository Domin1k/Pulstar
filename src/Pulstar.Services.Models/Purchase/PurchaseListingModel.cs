namespace Pulstar.Services.Models.Purchase
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Pulstar.Common.Interfaces;
    using Pulstar.DataModels;

    public class PurchaseListingModel : IMapFrom<Purchase>, IHaveCustomMapping
    {
        public long UniqueCode { get; set; }

        public DateTime Date { get; set; }

        public string UserName { get; set; }

        public decimal Amount { get; set; }

        public List<PurchaseHistoryProduct> Products { get; set; }

        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<Purchase, PurchaseListingModel>()
                .ForMember(p => p.UserName, opts => opts.MapFrom(p => p.User.UserName))
                .ForMember(p => p.Products, opts => opts.MapFrom(p => p.Products.Select(c => new PurchaseHistoryProduct
                {
                    ProductModel = c.Model,
                    ProductTitle = c.Title,
                })));
        }
    }
}
