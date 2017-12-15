namespace Pulstar.Common.Extensions
{
    using System.Linq;

    public static class ModelValidations
    {
        public static bool ContainsNullStrings<T>(this T model)
        {
            return model.GetType().GetProperties()
                .Where(pi => pi.GetValue(model) is string)
                .Select(pi => (string)pi.GetValue(model))
                .Any(value => string.IsNullOrEmpty(value));
        }
    }
}
