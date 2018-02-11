namespace Pulstar.Common.Constants
{
    public class ServiceErrorsConstants
    {
        public const string InvalidDiscountRange = "Discount must be in range [1...99]";
        public const string RequiredFields = "Required fields are missing";
        public const string CategoryDoesNotExist = "Category with id {0} does not exist.";
        public const string ProductIdDoesNotExist = "Product with id {0} does not exist.";
        public const string ProductDoesNotExist = "Product does not exists.";
        public const string CategoryExists = "Category {0} exists.";
        public const string AddressRequired = "Address is required!";
        public const string InvalidOrMissingProduct = "Products are missing or invalid in the database!";
        public const string ProductCartPriceMismatch = "There is mismatch between cart products prices and real product prices!";
        public const string UserInsufficientFunds = "User {0} does not have enough funds to perform this purchase.";
        public const string UserNotFound = "User not found!";
        public const string UserNameNotFound = "User {0} not found!";
        public const string InvalidFunds = "Funds must be equal or greater than zero.";
        public const string UserDoesNotHaveAnyPaymentMethods = "{0} does not have any payment methods available. Please add payment method and try again.";
        public const string InvalidDeposit = "{0} cannot be less or equal to 0.";
        public const string AddToRoleFailed = "Add to {0} for {1} failed.";
        public const string UserNameOrRoleNotExists = "{0}/{1} does not exists!";
        public const string InvalidUserCC = "User already has this CC added.";
        public const string InvalidUserInput = "Invalid user input! {0} must be a valid CC number, {1} must be valid CVV and {2} must NOT be past date.";
    }
}
