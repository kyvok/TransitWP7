//TODO: copyright info

namespace TransitWP7.BingMapsRestApi
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Text;
    using Microsoft.Phone.Controls.Maps;

    //TODO: this needs review
    public class LocationByPoint
    {
        public Point Point { get; set; }
        public List<EntityType> IncludeEntityTypes { get; set; } //TODO: this only supports a subset of the actual enum

        public LocationByPoint(Point point)
        {
            this.Point = point;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    
    //TODO: this needs review
    public class LocationQueryParameters
    {
        public string Query { get; set; }

        public LocationQueryParameters(string query)
        {
            this.Query = query;
        }
    }

    /// <summary>
    /// Represents the Route resources query parameters as defined in http://msdn.microsoft.com/en-us/library/ff701717.aspx
    /// </summary>
    public class RouteQueryParameters
    {
        public RouteQueryParameters(Point start, Point end)
        {
            if (this.Waypoints == null)
            {
                this.Waypoints = new List<Point>();
            }

            this.Waypoints.Add(start);
            this.Waypoints.Add(end);
        }

        public List<Point> Waypoints { get; set; }
        public List<AvoidType> Avoid { get; set; }
        public int? DistanceBeforeFirstTurn { get; set; }
        public int? Heading { get; set; }
        public OptimizeFor? Optimize { get; set; }
        public RoutePathOutput? RoutePathOutput { get; set; }
        public List<double> Tolerances { get; set; }
        public DistanceUnit? DistanceUnit { get; set; }
        public DateTime? DateTime { get; set; }
        public TimeType? TimeType { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            for (int i = 0; i < Waypoints.Count; i++)
            {
                builder.Append("&wp.");
                builder.Append(i);
                builder.Append("=");
                builder.Append(Waypoints[i]);
            }

            if (Avoid != null && Avoid.Count != 0)
            {
                builder.Append("&avoid=");
                bool moreThanOne = false;
                for (int i = 0; i < Avoid.Count; i++)
                {
                    if (moreThanOne)
                    {
                        builder.Append(",");
                    }

                    builder.Append(Avoid[i]);
                    moreThanOne = true;
                }
            }

            if (DistanceBeforeFirstTurn.HasValue)
            {
                builder.Append("&dbft=");
                builder.Append(DistanceBeforeFirstTurn);
            }

            if (Heading.HasValue)
            {
                builder.Append("&hd=");
                builder.Append(Heading);
            }

            if (Optimize.HasValue)
            {
                builder.Append("&optmz=");
                builder.Append(Optimize);
            }

            if (RoutePathOutput.HasValue)
            {
                builder.Append("&rpo=");
                builder.Append(RoutePathOutput);
            }

            if (Tolerances != null && Tolerances.Count != 0)
            {
                builder.Append("&tl=");
                bool moreThanOne = false;
                for (int i = 0; i < Tolerances.Count; i++)
                {
                    if (moreThanOne)
                    {
                        builder.Append(",");
                    }

                    builder.Append(Tolerances[i].ToString("G9"));
                    moreThanOne = true;
                }
            }

            if (DistanceUnit.HasValue)
            {
                builder.Append("&du=");
                builder.Append(DistanceUnit);
            }

            if (DateTime.HasValue)
            {
                builder.Append("&dt=");
                builder.Append(DateTime);
            }

            if (TimeType.HasValue)
            {
                builder.Append("&tt=");
                builder.Append(TimeType);
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// Represents the Transit resources query parameters as defined in http://msdn.microsoft.com/en-us/library/ff701717.aspx
    /// It adds the MaxSolutions parameters, which only applies to Transit, and forces assignement to DateTime and TimeType
    /// properties as they are required parameters for Transit.
    /// </summary>
    public class TransitQueryParameters : RouteQueryParameters
    {
        public TransitQueryParameters(Point start, Point end)
            : this(start, end, System.DateTime.Now, BingMapsRestApi.TimeType.Departure)
        {
        }

        public TransitQueryParameters(Point start, Point end, DateTime dateTime, TimeType timeType)
            : base(start, end)
        {
            this.DateTime = dateTime;
            this.TimeType = timeType;
        }

        public int? MaxSolutions { get; set; } //maxSolns

        public override string ToString()
        {
            if (this.MaxSolutions.HasValue)
            {
                return base.ToString() + "&maxSolns=" + this.MaxSolutions;
            }
            else
            {
                return base.ToString();
            }
        }
    }

    /// <summary>
    /// Represents the UserContext query parameters as defined in http://msdn.microsoft.com/en-us/library/ff701704.aspx
    /// </summary>
    public class UserContextParameters
    {
        public LocationRect MapView { get; set; } //mv
        public GeoCoordinate UserLocation { get; set; } //ul

        public UserContextParameters() { }

        public UserContextParameters(LocationRect mapView)
            : this(mapView, null)
        {
        }

        public UserContextParameters(GeoCoordinate userLocation)
            : this(null, userLocation)
        {
        }

        public UserContextParameters(LocationRect mapView, GeoCoordinate userLocation)
        {
            this.MapView = mapView;
            this.UserLocation = userLocation;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (this.MapView != null)
            {
                builder.Append("&mv=");
                builder.Append(this.MapView.AsBingMapsBoundingBox());
            }

            if (this.UserLocation != null)
            {
                builder.Append("&ul=");
                builder.Append(this.UserLocation.AsBingMapsPoint());
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// Represents the Output query parameters as defined in http://msdn.microsoft.com/en-us/library/ff701701.aspx
    /// </summary>
    public class OutputParameters
    {
        public OutputFormat? OutputFormat { get; set; } //o
        public bool? SuppressStatus { get; set; } //ss

        public OutputParameters() { }

        public OutputParameters(OutputFormat format)
        {
            this.OutputFormat = format;
        }

        public OutputParameters(bool suppressStatus)
        {
            this.SuppressStatus = suppressStatus;
        }

        public OutputParameters(OutputFormat format, bool suppressStatus)
        {
            this.OutputFormat = format;
            this.SuppressStatus = suppressStatus;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (this.OutputFormat.HasValue)
            {
                builder.Append("&o=");
                builder.Append(this.OutputFormat);
            }

            if (this.SuppressStatus.HasValue)
            {
                builder.Append("&ss=");
                builder.Append(this.SuppressStatus);
            }

            return builder.ToString();
        }
    }

    /// <summary>
    /// Represents the BingMapsKey query parameters, required to authenticate access to BingMaps.
    /// </summary>
    public class KeyParameter
    {
        public string Key { get; private set; }

        public KeyParameter(string key)
        {
            this.Key = key;
        }

        public override string ToString()
        {
            return string.Format("&key={0}", this.Key);
        }
    }

    public enum OutputFormat
    {
        Xml,
        Json //default
    }

    public enum TravelMode
    {
        Driving, //default if unspecified in resource path for Routes resource
        Walking,
        Transit
    }

    public enum OptimizeFor
    {
        Distance,
        Time, //default
        TimeWithTraffic
    }

    public enum RoutePathOutput
    {
        None, //default
        Points
    }

    public enum DistanceUnit
    {
        Kilometer, //default
        Mile
    }

    public enum TimeType
    {
        Arrival,
        Departure, //default
        LastArrivalTime
    }

    public enum AvoidType
    {
        Highways,
        Tolls,
        MinimizeHighways,
        MinimizeTolls
    }
}
