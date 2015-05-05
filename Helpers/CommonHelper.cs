using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Helpers
{
    public static class CommonHelper
    {
        public static T Clone<T>(this T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T) formatter.Deserialize(ms);
            }
        }

        public static T CastObject<T>(this object obj)
        {
            if (obj is T)
                return (T)obj;

            try
            {
                return (T) Convert.ChangeType(obj, typeof(T));
            }
            catch(InvalidCastException)
            {
                return default(T);
            }
        }
    }
}

