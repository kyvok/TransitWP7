namespace GoogleApisLib.GoogleMapsApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public static class GMapsHelper
    {
        public static DirectionsResult Decode(string x)
        {
            var result = JsonConvert.DeserializeObject<DirectionsResult>(x);
            return result;
        }
    }

    public enum DirectionsStatus
    {
        INVALID_REQUEST,
        MAX_WAYPOINTS_EXCEEDED,
        NOT_FOUND,
        OK,
        OVER_QUERY_LIMIT,
        REQUEST_DENIED,
        UNKNOWN_ERROR,
        ZERO_RESULTS
    }

    public enum TravelMode
    {
        BICYCLING,
        DRIVING,
        TRANSIT,
        WALKING
    }

    public enum UnitSystem
    {
        IMPERIAL,
        METRIC
    }

    public enum VehiculeType
    {
        RAIL,
        METRO_RAIL,
        SUBWAY,
        TRAM,
        MONORAIL,
        HEAVY_RAIL,
        COMMUTER_TRAIN,
        HIGH_SPEED_TRAIN,
        BUS,
        INTERCITY_BUS,
        TROLLEYBUS,
        SHARE_TAXI,
        FERRY,
        CABLE_CAR,
        GONDOLA_LIFT,
        FUNICULAR,
        OTHER
    }

    public class Polyline
    {
        public string points { get; set; }

        public LatLng[] DecodePolyline()
        {
            List<LatLng> poly = new List<LatLng>();
            int index = 0;
            int len = this.points.Length;
            int lat = 0;
            int lng = 0;

            while (index < len)
            {
                int b;
                int shift = 0;
                int result = 0;
                do
                {
                    b = this.points[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                }
                while (b >= 0x20);

                int dlat = (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                lat += dlat;

                shift = 0;
                result = 0;
                do
                {
                    b = this.points[index++] - 63;
                    result |= (b & 0x1f) << shift;
                    shift += 5;
                }
                while (b >= 0x20);

                int dlng = (result & 1) != 0 ? ~(result >> 1) : (result >> 1);
                lng += dlng;

                LatLng p = new LatLng
                    {
                        lat = lat / 1E5,
                        lng = lng / 1E5
                    };
                poly.Add(p);
            }

            return poly.ToArray();
        }
    }

    public class Distance
    {
        public string text { get; set; }

        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }

        public int value { get; set; }
    }

    public class Time
    {
        public string text { get; set; }

        public string time_zone { get; set; }

        public long value { get; set; }
    }

    public class LatLng
    {
        public double lat { get; set; }

        public double lng { get; set; }
    }

    public class LatLngBounds
    {
        public LatLng southwest { get; set; }

        public LatLng northeast { get; set; }
    }

    public class DirectionsResult
    {
        public DirectionsRoute[] routes { get; set; }

        public string status { get; set; }
    }

    public class DirectionsRoute
    {
        public LatLngBounds bounds { get; set; }

        public string copyrights { get; set; }

        public DirectionsLeg[] legs { get; set; }

        public Polyline overview_polyline { get; set; }

        public string[] warnings { get; set; }

        public int[] waypoint_order { get; set; }
    }

    public class DirectionsLeg
    {
        public Time arrival_time { get; set; }

        public Time departure_time { get; set; }

        public Distance distance { get; set; }

        public Duration duration { get; set; }

        public string end_address { get; set; }

        public LatLng end_location { get; set; }

        public string start_address { get; set; }

        public LatLng start_location { get; set; }

        public DirectionsStep[] steps { get; set; }

        public LatLng[] via_waypoints { get; set; }
    }

    public class DirectionsStep
    {
        public Distance distance { get; set; }

        public Duration duration { get; set; }

        public LatLng end_location { get; set; }

        public string instructions { get; set; }

        public string html_instructions { get; set; }

        public LatLng[] path { get; set; }

        public LatLng start_location { get; set; }

        public DirectionsStep[] steps { get; set; }

        public DirectionsStep[] sub_steps { get; set; }

        public TransitDetails transit { get; set; }

        public TravelMode travel_mode { get; set; }
    }

    public class TransitDetails
    {
        public TransitStop arrival_stop { get; set; }

        public Time arrival_time { get; set; }

        public TransitStop departure_stop { get; set; }

        public Time departure_time { get; set; }

        public string headsign { get; set; }

        public int headway { get; set; }

        public TransitLine line { get; set; }

        public int num_stops { get; set; }
    }

    public class TransitStop
    {
        public LatLng location { get; set; }

        public string name { get; set; }
    }

    public class TransitLine
    {
        public TransitAgency[] agencies { get; set; }

        public string color { get; set; }

        public string icon { get; set; }

        public string name { get; set; }

        public string short_name { get; set; }

        public string text_color { get; set; }

        public string url { get; set; }

        public TransitVehicle vehicle { get; set; }
    }

    public class TransitAgency
    {
        public string name { get; set; }

        public string phone { get; set; }

        public string url { get; set; }
    }

    public class TransitVehicle
    {
        public string icon { get; set; }

        public string local_icon { get; set; }

        public string name { get; set; }

        public VehiculeType type { get; set; }
    }
}