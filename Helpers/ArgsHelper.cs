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
			var arg = args.SingleOrDefault(argument => argument.StartsWith("/" + argName));
			if(arg == null)
				throw new ArgumentException(string.Format("Need argument {0}.", argName));

			return arg.Split(':').Last().CastObject<T>();
        }

		public static bool ContainsArg(this string[] args, string argName)
		{
			return args.SingleOrDefault(arg => arg.StartsWith("/" + argName)) != null;
		}
    }
}

