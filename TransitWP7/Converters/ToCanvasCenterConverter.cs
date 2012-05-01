namespace TransitWP7.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    public class ToCanvasCenterConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            var objectSize = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
            var canvasSize = System.Convert.ToDouble(parameter, CultureInfo.InvariantCulture);
            return (canvasSize - objectSize) / 2;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}