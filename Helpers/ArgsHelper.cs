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

            return value.CastObject<T>();
        }

		public static bool ContainsArg(this string[] args, string argName)
		{
			return args.SingleOrDefault(arg => arg.StartsWith("/" + argName)) != null;
		}
    }
}

