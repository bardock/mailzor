using System.Collections.Generic;
using System.Linq;

namespace EmailModule
{
    using System;
    using System.Globalization;

    internal static class Invariant
    {
        public static void IsNotNull(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        public static void IsNotBlank(string target, string parameterName)
        {
            if (string.IsNullOrEmpty(target))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, "\"{0}\" cannot be blank.", parameterName));
            }
        }

        public static void IsNotEmpty<T>(IEnumerable<KeyValuePair<string, T>> templates, string message)
        {
            if (!templates.Any())
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentUICulture, message));
            }
        }
    }
}