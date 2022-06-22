using System;
using System.Globalization;
using Xamarin.Forms;

namespace Go_Study_Mobile.Styles.Converters
{
    internal class FontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double fontsize = System.Convert.ToDouble(parameter);

            double screenresolution = System.Convert.ToDouble(Application.Current.Resources["ScreenResolution"]);

            return fontsize * screenresolution / 6641.12189317438;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
