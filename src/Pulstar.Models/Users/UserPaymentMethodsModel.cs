namespace Pulstar.Models.Users
{
    using System;
    using AutoMapper;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Helpers;
    using Pulstar.Common.Interfaces;
    using Pulstar.DataModels;

    public class UserPaymentMethodsModel : IMapFrom<CreditCard>, IHaveCustomMapping
    {
        public int Id { get; set; }

        public string CreditCardNumber { get; set; }
        
        public string CVV { get; set; }

        public bool IsActive { get; set; }

        public string CardHolderName { get; set; }

        public DateTime ExpirationDate { get; set; }

        public CreditCardType CardType { get; set; }

        public void ConfigureMapping(Profile profile)
        {
            profile.CreateMap<CreditCard, UserPaymentMethodsModel>()
                .ForMember(c => c.CreditCardNumber, opts => opts.MapFrom(c => CreditCardHelper.Decrypt(c.CreditCardNumber)));
        }
    }
}
