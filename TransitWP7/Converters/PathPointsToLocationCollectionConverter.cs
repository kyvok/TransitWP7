namespace TransitWP7.Converters
{
    using System.Globalization;
    using System.Windows.Data;
    using BingApisLib.BingMapsRestApi;
    using Microsoft.Phone.Controls.Maps;
    using TransitWP7.Model;

    public class PathPointsToLocationCollectionConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            var pathPoints = (Point[])value;

            var locations = new LocationCollection();
            foreach (var pathPoint in pathPoints)
            {
                locations.Add(pathPoint.AsGeoCoordinate());
            }

            return locations;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}