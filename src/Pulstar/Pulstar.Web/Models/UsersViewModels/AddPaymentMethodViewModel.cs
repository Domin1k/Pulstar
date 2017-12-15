namespace Pulstar.Web.Models.UsersViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AddPaymentMethodViewModel
    {
        [CreditCard]
        [DataType(DataType.CreditCard)]
        public string CreditCardNumber { get; set; }

        public string CVV { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }
    }
}
