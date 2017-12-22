namespace Pulstar.Models.Users
{
    using System;
    using Pulstar.Common.Enums;
    using Pulstar.Common.Interfaces;
    using Pulstar.Data.Models;

    public class UserPaymentMethodsModel : IMapFrom<CreditCard>
    {
        public string CreditCardNumber { get; set; }
        
        public string CVV { get; set; }

        public bool IsActive { get; set; }

        public string CardHolderName { get; set; }

        public DateTime ExpirationDate { get; set; }

        public CreditCardType CardType { get; set; }
    }
}
