namespace Pulstar.Common.Extensions
{
    using System;
    using System.Linq;
    using System.Reflection;

    public static class ModelValidations
    {
        public static bool ContainsNullStrings<T>(this T model)
        {
            foreach (PropertyInfo pi in model.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(string))
                {
                    string value = (string)pi.GetValue(model);
                    if (string.IsNullOrEmpty(value))
                    {
                        return true;
                    }
                }
            }

            return false;
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
