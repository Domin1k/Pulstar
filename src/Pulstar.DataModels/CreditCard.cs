namespace Pulstar.DataModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Pulstar.Common.Enums;

    public class CreditCard : BaseEntity, IValidatableObject
    {
        [Required]
        [MinLength(12)]
        [MaxLength(19)]
        public string CreditCardNumber { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(4)]
        public string CVV { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public string CardHolderName { get; set; }

        public DateTime ExpirationDate { get; set; }

        public CreditCardType CardType { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public User Owner { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResult = new List<ValidationResult>();
            if (ExpirationDate < DateTime.UtcNow)
            {
                validationResult.Add(new ValidationResult("ExpirationDate cannot be in the past."));
            }

            return validationResult;
        }
    }
}
