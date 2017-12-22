namespace Pulstar.Common.Extensions
{
    using System;
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

        public static void ThrowIfNull(this string str)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrWhiteSpace(str) || str?.Trim()?.Length <= 0)
            {
                throw new InvalidOperationException($"{nameof(str)} cannot be null.");
            }
        }
    }
}
