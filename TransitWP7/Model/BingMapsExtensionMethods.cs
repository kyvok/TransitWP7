namespace TransitWP7.Model
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using BingApisLib.BingMapsRestApi;
    using Microsoft.Phone.Controls.Maps;

    public static class BingMapsExtensionMethods
    {
        public static LocationRect AsLocationRect(this BoundingBox bb)
        {
            if (bb == null)
            {
                throw new ArgumentNullException("bb");
            }

            var locRect = new LocationRect(bb.NorthLatitude, bb.WestLongitude, bb.SouthLatitude, bb.EastLongitude);
            locRect.Northeast = FixGeoCoordinate(locRect.Northeast);
            locRect.Northwest = FixGeoCoordinate(locRect.Northwest);
            locRect.Southeast = FixGeoCoordinate(locRect.Southeast);
            locRect.Southwest = FixGeoCoordinate(locRect.Southwest);
            return locRect;
        }

        public static GeoCoordinate AsGeoCoordinate(this Point point)
        {
            if (point == null)
            {
                throw new ArgumentNullException("point");
            }

            return FixGeoCoordinate(new GeoCoordinate(point.Latitude, point.Longitude));
        }

        public static IEnumerable<Route> GetRoutes(this Response response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            return response.ResourceSets[0].Resources.OfType<Route>().AsEnumerable();
        }

        public static IEnumerable<Location> GetLocations(this Response response)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            return response.ResourceSets[0].Resources.OfType<Location>().AsEnumerable();
        }

        // This is to reduce the number of NaN in the serialized file which degrades deserialization perf.
        private static GeoCoordinate FixGeoCoordinate(GeoCoordinate geoCoordinate)
        {
            geoCoordinate.Altitude = 0;
            geoCoordinate.Course = 0;
            geoCoordinate.HorizontalAccuracy = 0;
            geoCoordinate.Speed = 0;
            geoCoordinate.VerticalAccuracy = 0;
            return geoCoordinate;
        }
    }
}
