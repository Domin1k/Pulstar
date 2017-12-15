namespace Pulstar.Common.Helpers
{
    using System;
    using System.Text.RegularExpressions;
    using Pulstar.Common.Enums;

    public static class CreditCardHelper
    {
        public static CreditCardType GetCreditCardType(string creditCardNumber)
        {
            string isVisa = "^4[0-9]{12}(?:[0-9]{3})?$";

            if (Regex.IsMatch(creditCardNumber, isVisa))
            {
                return CreditCardType.Visa;
            }

            throw new NotSupportedException($"Sorry, but our application supports only VISA.");
        }
    }
}
