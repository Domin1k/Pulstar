namespace Pulstar.Web.Infrastructure.Constants
{
    public class TempMessages
    {
        /*Errors*/
        public const string InvalidProduct = "Invalid product.";
        public const string AlreadyInCart = "You already have this product in your cart!";
        public const string NoProductsError = "There are no products for {0}";
        public const string GeneralInvalidInputData = "Some of the inputs contain invalid data.";
        public const string InvalidDiscount = "Discount must be in range [1..99] and product must be existing.";
        public const string MaximumImageSizeExceeded = "Sorry, but maximum size for pictures in {0}MB";

        /*Success*/
        public const string AddedSuccefully = "Added to cart successfully";
        public const string ThankYouPurchase = "Thank you for your purchase!";
        public const string SuccessDeposit = "Successfully deposited funds.";
        public const string AddedPaymentMethodSuccess = "You have succesfully added payment method to user.";
    }
}
