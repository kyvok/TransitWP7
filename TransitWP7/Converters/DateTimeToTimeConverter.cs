namespace TransitWP7.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;

    public class DateTimeToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime? datetime = value as DateTime?;
            if (datetime.Value == DateTime.MinValue)
            {
                return string.Empty;
            }
            else
            {
                return datetime.Value.ToString("t");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
