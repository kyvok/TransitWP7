//TODO: copyright info

namespace TransitWP7
{
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using Microsoft.Phone.Controls.Maps;
    using TransitWP7.BingMapsRestApi;

    public static class BingMapsExtensionMethods
    {
        public static BoundingBox AsBingMapsBoundingBox(this LocationRect locRect)
        {
            return new BoundingBox(locRect.South, locRect.West, locRect.North, locRect.East);
        }

        public static LocationRect AsLocationRect(this BoundingBox bb)
        {
            return new LocationRect(bb.NorthLatitude, bb.WestLongitude, bb.SouthLatitude, bb.EastLongitude);
        }

        public static Point AsBingMapsPoint(this GeoCoordinate geoCoord)
        {
            return new Point(geoCoord.Latitude, geoCoord.Longitude);
        }

        public static GeoCoordinate AsGeoCoordinate(this Point point)
        {
            return new GeoCoordinate(point.Latitude, point.Longitude);
        }

        public static IEnumerable<Route> GetRoutes(this Response response)
        {
            return response.ResourceSets[0].Resources.OfType<Route>().AsEnumerable();
        }

        public static IEnumerable<Location> GetLocations(this Response response)
        {
            return response.ResourceSets[0].Resources.OfType<Location>().AsEnumerable();
        }
    }
}
