﻿namespace TransitWP7.Model
{
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using BingApisLib.BingMapsRestApi;
    using Microsoft.Phone.Controls.Maps;

    public static class BingMapsExtensionMethods
    {
        public static LocationRect AsLocationRect(this BoundingBox bb)
        {
            return new LocationRect(bb.NorthLatitude, bb.WestLongitude, bb.SouthLatitude, bb.EastLongitude);
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
