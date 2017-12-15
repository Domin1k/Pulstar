namespace Pulstar.Models.Users
{
    using System;
    using AutoMapper;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Interfaces;
    using Pulstar.Data.Models;

    public class UserPaymentMethodsModel : IMapFrom<CreditCard>, IHaveCustomMapping
    {
        public string CreditCardNumber { get; set; }
        
        public string CVV { get; set; }

        public bool IsActive { get; set; }

        public DateTime ExpirationDate { get; set; }

        public CreditCardType CardType { get; set; }
        
        public string Owner { get; set; }

        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<CreditCard, UserPaymentMethodsModel>()
                .ForMember(c => c.Owner, opts => opts.MapFrom(m => m.Owner.UserName));
        }
    }
}
