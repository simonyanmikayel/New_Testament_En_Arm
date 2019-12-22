using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace ThisApp.Helpers
{
    public class EnumAndNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            object o = null;
            if (value != null)
            {
                if (targetType.IsEnum)
                {
                    // convert int to enum
                    o = Enum.ToObject(targetType, value);
                }
                else if (value.GetType().IsEnum)
                {
                    // convert enum to int
                    o = System.Convert.ChangeType(
                        value,
                        Enum.GetUnderlyingType(value.GetType()),
                        CultureInfo.InvariantCulture);
                }
            }

            return o;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            // perform the same conversion in both directions
            return Convert(value, targetType, parameter, language);
        }
    }
}
