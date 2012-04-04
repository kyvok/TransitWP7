namespace TransitWP7.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ItineraryStepsToImageConverter : IValueConverter
    {
        // TODO: Complete this class properly and move the URI strings to static const.

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
                        return new Uri("/images/light/walk.100x100.png", UriKind.Relative);
                    case 'B':
                        return new Uri("/images/light/bus.100x100.png", UriKind.Relative);
                    case 'T':
                        return new Uri("/images/light/rail.100x100.png", UriKind.Relative);
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
