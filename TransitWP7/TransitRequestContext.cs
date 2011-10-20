//TODO: copyright info

namespace TransitWP7
{
    using System.Device.Location;

    public static class TransitRequestContext
    {
        public static GeoCoordinate UserLocation { get; set; }
        public static GeoCoordinate StartLocation { get; set; }
        public static GeoCoordinate EndLocation { get; set; }
        public static TransitWP7.BingMapsRestApi.TimeType TimeType { get; set; }
    }
}