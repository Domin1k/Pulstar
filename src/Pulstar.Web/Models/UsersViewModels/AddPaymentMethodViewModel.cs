namespace Pulstar.Web.Models.UsersViewModels
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AddPaymentMethodViewModel
    {
        [CreditCard]
        [DataType(DataType.CreditCard)]
        public string CreditCardNumber { get; set; }

        [Required]
        [RegularExpression("[0-9]{3,4}", ErrorMessage = "CVV is invalid. Must be between 3 and 4 numbers long.")]
        public string CVV { get; set; }

        [DataType(DataType.Date)]
        public DateTime ExpirationDate { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string CardHolderName { get; set; }
    }
}
