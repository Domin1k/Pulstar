namespace Pulstar.Common.Helpers
{
    using System;
    using Pulstar.Common.Enums;

    public static class UrlHelper
    {
        public static string GenerateUrl(CategoryType categoryType)
        {
            switch (categoryType)
            {
                case CategoryType.Game: return "Games";
                case CategoryType.Console: return "Consoles";
                case CategoryType.Accessory: return "Accessories";
            }

            throw new NotSupportedException($"{categoryType} not supportet");
        }
    }
}
