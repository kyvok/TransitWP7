namespace TransitWP7.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class SequentialValueConverter : List<IValueConverter>, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            object returnValue = value;
            
            foreach (IValueConverter converter in this)
            {
                returnValue = converter.Convert(returnValue, targetType, parameter, culture);
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
