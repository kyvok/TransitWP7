using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Text;
using Microsoft.Phone.Controls.Maps;

namespace BingApisLib.BingMapsRestApi
{
    public enum OutputFormat
    {
        Xml,

        // default
        Json
    }

    public enum TravelMode
    {
        // default if unspecified in resource path for Routes resource
        Driving,
        Walking,
        Transit
    }

    public enum OptimizeFor
    {
        Distance,

        // default
        Time,
        TimeWithTraffic
    }

    public enum RoutePathOutput
    {
        // default
        None,
        Points
    }

    public enum DistanceUnit
    {
        // default
        Kilometer,
        Mile
    }

    public enum TimeType
    {
        Arrival,

        // default
        Departure,
        LastAvailable
    }

    public enum AvoidType
    {
        Highways,
        Tolls,
        MinimizeHighways,
        MinimizeTolls
    }

    // TODO: this needs review
    public class LocationByPoint
    {
        public LocationByPoint(Point point)
        {
            this.Point = point;
        }

        public Point Point { get; set; }

        // TODO: this only supports a subset of the actual enum
        public List<EntityType> IncludeEntityTypes { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }
    }
    
    // TODO: this needs review
    public class LocationQueryParameters
    {
        public LocationQueryParameters(string query)
        {
            this.Query = query;
        }

        public string Query { get; set; }
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

            for (int i = 0; i < this.Waypoints.Count; i++)
            {
                builder.Append("&wp.");
                builder.Append(i);
                builder.Append("=");
                builder.Append(this.Waypoints[i]);
            }

            if (this.Avoid != null && this.Avoid.Count != 0)
            {
                builder.Append("&avoid=");
                bool moreThanOne = false;
                for (int i = 0; i < this.Avoid.Count; i++)
                {
                    if (moreThanOne)
                    {
                        builder.Append(",");
                    }

                    builder.Append(this.Avoid[i]);
                    moreThanOne = true;
                }
            }

            if (this.DistanceBeforeFirstTurn.HasValue)
            {
                builder.Append("&dbft=");
                builder.Append(this.DistanceBeforeFirstTurn);
            }

            if (this.Heading.HasValue)
            {
                builder.Append("&hd=");
                builder.Append(this.Heading);
            }

            if (this.Optimize.HasValue)
            {
                builder.Append("&optmz=");
                builder.Append(this.Optimize);
            }

            if (this.RoutePathOutput.HasValue)
            {
                builder.Append("&rpo=");
                builder.Append(this.RoutePathOutput);
            }

            if (this.Tolerances != null && this.Tolerances.Count != 0)
            {
                builder.Append("&tl=");
                bool moreThanOne = false;
                for (int i = 0; i < this.Tolerances.Count; i++)
                {
                    if (moreThanOne)
                    {
                        builder.Append(",");
                    }

                    builder.Append(this.Tolerances[i].ToString("G9"));
                    moreThanOne = true;
                }
            }

            if (this.DistanceUnit.HasValue)
            {
                builder.Append("&du=");
                builder.Append(this.DistanceUnit);
            }

            if (this.DateTime.HasValue)
            {
                builder.Append("&dt=");
                builder.Append(this.DateTime);
            }

            if (this.TimeType.HasValue)
            {
                builder.Append("&tt=");
                builder.Append(this.TimeType);
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

        // maxSolns
        public int? MaxSolutions { get; set; }

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
        public UserContextParameters()
        {
        }

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

        // mv
        public LocationRect MapView { get; set; }

        // ul
        public GeoCoordinate UserLocation { get; set; }

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
        public OutputParameters()
        {
        }

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

        public OutputFormat? OutputFormat { get; set; } // o

        public bool? SuppressStatus { get; set; } // ss

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
        public KeyParameter(string key)
        {
            this.Key = key;
        }

        public string Key { get; private set; }

        public override string ToString()
        {
            return string.Format("&key={0}", this.Key);
        }
    }
}
