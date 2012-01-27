using System;
using System.Globalization;
using System.Windows.Data;

namespace TransitWP7.Converters
{
    public class TravelDurationConverter : IValueConverter
    {
        /// <summary>
        /// Converts a time in seconds to a time in minutes.
        /// </summary>
        /// <param name="value">The value to convert from</param>
        /// <param name="targetType">Type targeted</param>
        /// <param name="parameter">Optional parameters</param>
        /// <param name="culture">Culture to use</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)((double)value / 60)).ToString(CultureInfo.InvariantCulture) + " min";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
