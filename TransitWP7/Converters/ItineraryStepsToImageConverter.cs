﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
                        return new Uri("/images/walk.png", UriKind.Relative);
                    case 'B':
                        return new Uri("/images/bus.png", UriKind.Relative);
                    case 'T':
                        return new Uri("/images/rail.png", UriKind.Relative);
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
