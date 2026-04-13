using System;
using System.Collections.Generic;
using System.Text;
using InventorySystem_MAUI.Helper;
using System.Globalization;

namespace InventorySystem_MAUI.Converters
{


    public class UriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            Conection.BaseURI + value.ToString();



        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString().Remove(0, Conection.BaseURI.Length);
        }
    }

}
