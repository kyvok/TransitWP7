//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;

    public static class TransitRequestContext
    {
        public static GeoCoordinate UserLocation { get; set; }
        public static GeoCoordinate StartLocation { get; set; }
        public static GeoCoordinate EndLocation { get; set; }
        public static DateTime DateTime { get; set; }
        public static TimeCondition TimeType { get; set; }

        public static IEnumerable<string> EnumValues { get { return Enum<TimeCondition>.GetNames(); } }
    }
}