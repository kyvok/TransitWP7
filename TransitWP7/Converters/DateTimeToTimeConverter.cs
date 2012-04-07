namespace TransitWP7.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class DateTimeToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var datetime = (DateTime?)value;
            return datetime.Value == DateTime.MinValue ? string.Empty : datetime.Value.ToString("t", culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
