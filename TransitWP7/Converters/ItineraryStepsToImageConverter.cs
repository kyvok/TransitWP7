using System;
using System.Globalization;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace TransitWP7.Converters
{
    public class ItineraryStepsToImageConverter : IValueConverter
    {
        // TODO: Complete this class properly and move the URI strings to static const.
        // TODO: Take a parameter to prevent repeting walking over and over

        /// <summary>
        /// Converts a step description to the corresponding pictogram uri.
        /// </summary>
        /// <param name="value">The value to convert from</param>
        /// <param name="targetType">Type targeted</param>
        /// <param name="parameter">Optional parameters</param>
        /// <param name="culture">Culture to use</param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var iconType = value as string;
            if (iconType != null)
            {
                switch (iconType[0])
                {
                    case 'W':
                        return new Uri("/images/walk_lo.png", UriKind.Relative);
                    case 'B':
                        return new Uri("/images/bus_lo.png", UriKind.Relative);
                    case 'T':
                        break;
                    case 'M':
                        break;
                    default:
                        return null;
                }
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
