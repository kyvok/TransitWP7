namespace BingApisLib
{
    using System.Device.Location;
    using Microsoft.Phone.Controls.Maps;

    public static class BingMapsExtensionMethods
    {
        public static BingMapsRestApi.BoundingBox AsBingMapsBoundingBox(this LocationRect locRect)
        {
            return new BingMapsRestApi.BoundingBox(locRect.South, locRect.West, locRect.North, locRect.East);
        }

        public static BingMapsRestApi.Point AsBingMapsPoint(this GeoCoordinate geoCoord)
        {
            return new BingMapsRestApi.Point(geoCoord.Latitude, geoCoord.Longitude);
        }
    }
}
