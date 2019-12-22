using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThisApp.Extensions;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace ThisApp.Helpers
{
    public static class LocalSettings
    {
        private static ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public static IPropertySet Values { get { return localSettings.Values; } }
        public static int GetInt(string propertyName, int defaultVal)
        {
            return Values[propertyName].GetInt(defaultVal);
        }
        public static T GetEnum<T>(string propertyName, T defaultVal)
        {
            return Values[propertyName].GetEnum<T>(defaultVal);
        }
    }
}
