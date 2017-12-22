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

        public static string Encrypt(string ccNum)
        {
            return Convert.ToString((Convert.ToInt64(ccNum) * 13) + 43);
        }

        public static string Decrypt(string encryptedCcNum)
        {
            return Convert.ToString((Convert.ToInt64(encryptedCcNum) - 43) / 13);
        }

        public static string ReplaceCreditCardUI(string creditCardNumber)
        {
            return creditCardNumber.Replace(creditCardNumber.Substring(0, creditCardNumber.Length - 3), new string('*', creditCardNumber.Length - 3));
        }
    }
}
