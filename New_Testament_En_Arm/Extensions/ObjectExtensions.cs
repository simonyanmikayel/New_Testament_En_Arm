using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThisApp.Extensions
{
    public static class ObjectExtensions
    {
        public static int GetInt(this object o, int defaultVal)
        {
            if (o is int i)
                return i;
            else
                return defaultVal;
        }
        public static T GetEnum<T>(this object o, T defaultVal)
        {
            if ((o is int i) && Enum.IsDefined(typeof(T), i))
            {
                return (T)System.Convert.ChangeType(
                        o,
                        Enum.GetUnderlyingType(typeof(T)),
                        CultureInfo.InvariantCulture);
            }
            else
                return defaultVal;
        }
    }
}
