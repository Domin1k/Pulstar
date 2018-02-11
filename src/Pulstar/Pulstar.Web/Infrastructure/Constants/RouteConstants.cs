namespace Pulstar.Web.Infrastructure.Constants
{
    public class RouteConstants
    {
        public const string SearchTerm = "{searchTerm?}";

        public const string ProductsController = "products";

        public const string Games = "games/{game}/{criteria?}/{order?}";

        public const string Consoles = "consoles/{console}/{criteria?}/{order?}";

        public const string Accessories = "accessories/{accessory}/{criteria?}/{order?}";

        public const string ProductsDetails = "details/{id}";
    }
}
