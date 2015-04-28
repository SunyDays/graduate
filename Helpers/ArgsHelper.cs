using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Helpers
{
    public static class ArgsHelper
    {
        public static T GetArgValue<T>(this string[] args, string argName)
        {
            object value = args.Single(arg => arg.StartsWith("/" + argName)).Split(':').Last();

            if (value is T)
                return (T)value;
            else
            {
                try
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch(InvalidCastException)
                {
                    return default(T);
                }
            }
        }
    }
}

