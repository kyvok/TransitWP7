namespace BingApisLib
{
    using System;
    using System.Device.Location;
    using Microsoft.Phone.Controls.Maps;

    public static class BingMapsExtensionMethods
    {
        public static BingMapsRestApi.BoundingBox AsBingMapsBoundingBox(this LocationRect locRect)
        {
            if (locRect == null)
            {
                throw new ArgumentNullException("locRect");
            }

            return new BingMapsRestApi.BoundingBox(locRect.South, locRect.West, locRect.North, locRect.East);
        }

        public static BingMapsRestApi.Point AsBingMapsPoint(this GeoCoordinate geoCoord)
        {
            if (geoCoord == null)
            {
                throw new ArgumentNullException("geoCoord");
            }

            return new BingMapsRestApi.Point(geoCoord.Latitude, geoCoord.Longitude);
        }
    }
}
