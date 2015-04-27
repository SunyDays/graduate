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
    }
}
