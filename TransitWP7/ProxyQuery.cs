//TODO: copyright info

namespace TransitWP7
{
    using System;
    using System.Linq;
    using TransitWP7.BingMapsRestApi;
    using TransitWP7.BingSearchRestApi;
    using System.Device.Location;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;

    /// <summary>
    /// Calls REST APIs and isolate their type mapping by converting to transitive types.
    /// </summary>
    public static class ProxyQuery
    {
        public static void GetLocationAddress(GeoCoordinate location, Action<ProxyQueryResult> callback, object userState)
        {
            var queryState = new QueryState
            {
                UserCallback = callback,
                UserState = userState
            };

            BingMapsQuery.GetLocationInfo(location, GetLocationInfoCallback, queryState);
        }

        public static void GetLocationsAndBusiness(string query, GeoCoordinate currentUserPosition, Action<ProxyQueryResult> callback, object userState)
        {
            var queryState = new QueryState
            {
                Query = query,
                UserLocation = currentUserPosition,
                UserState = userState,
                UserCallback = callback
            };

            BingMapsQuery.GetLocationsFromQuery(
                query,
                new UserContextParameters(currentUserPosition),
                GetLocationsFromQueryCallback,
                queryState);
        }

        public static void GetTransitDirections(GeoCoordinate startPoint, GeoCoordinate endPoint, DateTime dateTime, TimeCondition timeType, Action<ProxyQueryResult> callback, object userState)
        {
            var queryState = new QueryState
            {
                StartLocation = startPoint,
                EndLocation = endPoint,
                UserCallback = callback,
                UserState = userState
            };

            timeType = timeType != TimeCondition.Now ? timeType : TimeCondition.DepartingAt;

            BingMapsQuery.GetTransitRoute(startPoint, endPoint, dateTime, (TimeType)timeType, GetTransitDirectionsCallback, queryState);
        }

        private static void GetLocationInfoCallback(BingMapsQueryResult result)
        {
            var queryState = result.UserState as QueryState;

            var proxyQueryResult = new ProxyQueryResult() { UserState = queryState.UserState };
            if (result.Error == null)
            {
                foreach (var location in result.Response.GetLocations())
                {
                    if (proxyQueryResult.LocationDescriptions == null)
                    {
                        proxyQueryResult.LocationDescriptions = new List<LocationDescription>();
                    }

                    proxyQueryResult.LocationDescriptions.Add(new LocationDescription(location));
                }
            }
            else
            {
                proxyQueryResult.Error = result.Error;
            }

            queryState.UserCallback(proxyQueryResult);
        }

        private static void GetLocationsFromQueryCallback(BingMapsQueryResult result)
        {
            var queryState = result.UserState as QueryState;

            if (result.Error == null)
            {
                foreach (var location in result.Response.GetLocations())
                {
                    var locationDescription = new LocationDescription(location);

                    // ignore values farther than 80 miles. (same as phonebook API)
                    if (locationDescription.GeoCoordinate.GetDistanceTo(queryState.UserLocation) / 1600 > 80)
                    {
                        continue;
                    }

                    if (queryState.LocationDescriptions == null)
                    {
                        queryState.LocationDescriptions = new List<LocationDescription>();
                    }

                    queryState.LocationDescriptions.Add(locationDescription);
                }
            }
            else
            {
                queryState.SavedException = result.Error;
            }

            //chain querying for business
            BingSearchQuery.GetBusinessInfo(
                queryState.Query,
                queryState.UserLocation,
                GetBusinessFromQueryCallback,
                queryState);
        }

        private static void GetBusinessFromQueryCallback(BingSearchQueryResult result)
        {
            var queryState = result.UserState as QueryState;

            if (result.Error == null)
            {
                foreach (var pbResult in result.Response.Phonebook.Results)
                {
                    if (queryState.LocationDescriptions == null)
                    {
                        queryState.LocationDescriptions = new List<LocationDescription>();
                    }

                    queryState.LocationDescriptions.Add(new LocationDescription(pbResult));
                }
            }
            else
            {
                queryState.SavedException = result.Error;
            }

            //call user callback
            var proxyQueryResult = new ProxyQueryResult() { UserState = queryState.UserState };
            if (queryState.LocationDescriptions != null && queryState.LocationDescriptions.Count > 0)
            {
                proxyQueryResult.LocationDescriptions = queryState.LocationDescriptions;
            }
            else
            {
                if (queryState.SavedException != null)
                {
                    proxyQueryResult.Error = queryState.SavedException;
                }
                else
                {
                    proxyQueryResult.Error = new Exception("no results");
                }
            }

            queryState.UserCallback(proxyQueryResult);
        }

        private static void GetTransitDirectionsCallback(BingMapsQueryResult result)
        {
            var queryState = result.UserState as QueryState;

            if (result.Error == null)
            {
                foreach (var route in result.Response.GetRoutes())
                {
                    if (queryState.TransitDescriptions == null)
                    {
                        queryState.TransitDescriptions = new List<TransitDescription>();
                    }

                    queryState.TransitDescriptions.Add(new TransitDescription(route));
                }
            }
            else
            {
                queryState.SavedException = result.Error;
            }

            //chain immediately to get walking directions
            BingMapsQuery.GetWalkingRoute(queryState.StartLocation, queryState.EndLocation, GetWalkingDirectionsCallback, queryState);
        }

        private static void GetWalkingDirectionsCallback(BingMapsQueryResult result)
        {
            var queryState = result.UserState as QueryState;

            if (result.Error == null)
            {
                foreach (var route in result.Response.GetRoutes())
                {
                    var transitDescription = new TransitDescription(route);

                    // ignore more than 90 minute walks.
                    if (transitDescription.TravelDuration > 60 * 90)
                    {
                        continue;
                    }

                    if (queryState.TransitDescriptions == null)
                    {
                        queryState.TransitDescriptions = new List<TransitDescription>();
                    }

                    queryState.TransitDescriptions.Add(transitDescription);
                }
            }
            else
            {
                queryState.SavedException = result.Error;
            }

            //call user callback
            var proxyQueryResult = new ProxyQueryResult() { UserState = queryState.UserState };
            if (queryState.TransitDescriptions != null && queryState.TransitDescriptions.Count > 0)
            {
                proxyQueryResult.TransitDescriptions = queryState.TransitDescriptions;
            }
            else
            {
                if (queryState.SavedException != null)
                {
                    proxyQueryResult.Error = queryState.SavedException;
                }
                else
                {
                    proxyQueryResult.Error = new Exception("no results");
                }
            }

            queryState.UserCallback(proxyQueryResult);
        }

        private class QueryState
        {
            public string Query { get; set; }
            public GeoCoordinate UserLocation { get; set; }
            public GeoCoordinate StartLocation { get; set; }
            public GeoCoordinate EndLocation { get; set; }
            public List<LocationDescription> LocationDescriptions { get; set; }
            public List<TransitDescription> TransitDescriptions { get; set; }
            public Exception SavedException { get; set; }
            public Action<ProxyQueryResult> UserCallback { get; set; }
            public object UserState { get; set; }
        }
    }

    public class ProxyQueryResult
    {
        public object UserState { get; set; }
        public List<LocationDescription> LocationDescriptions { get; set; }
        public List<TransitDescription> TransitDescriptions { get; set; }
        public Exception Error { get; set; }
    }

    public enum TimeCondition
    {
        ArrivingAt,
        DepartingAt,
        LastArrivalTime,
        Now
    }

    public class TransitDescription
    {
        private Route route;
        private List<ItineraryStep> steps;

        public TransitDescription(Route route)
        {
            this.route = route;
            InitializeItinerarySteps();
        }

        private void InitializeItinerarySteps()
        {
            this.steps = new List<ItineraryStep>();

            foreach (var topLeg in this.route.RouteLegs[0].ItineraryItems)
            {
                this.steps.Add(new ItineraryStep(topLeg));

                if (topLeg.ChildItineraryItems != null)
                {
                    foreach (var childLeg in topLeg.ChildItineraryItems)
                    {
                        this.steps.Add(new ItineraryStep(childLeg));
                    }
                }
            }

            //TODO: need to add endpoint to the step list?
        }

        public Microsoft.Phone.Controls.Maps.LocationRect GetMapView()
        {
            return this.route.BoundingBox.AsLocationRect();
        }

        public Microsoft.Phone.Controls.Maps.MapPolyline GetMapPolyline()
        {
            var polyline = new Microsoft.Phone.Controls.Maps.MapPolyline()
            {
                StrokeLineJoin = System.Windows.Media.PenLineJoin.Round,
                Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Blue),
                StrokeThickness = 4,
                StrokeStartLineCap = System.Windows.Media.PenLineCap.Round,
                StrokeEndLineCap = System.Windows.Media.PenLineCap.Round,
                Locations = new Microsoft.Phone.Controls.Maps.LocationCollection(),
                Opacity = 0.7,
            };

            foreach (var point in this.route.RoutePaths[0].Line)
            {
                polyline.Locations.Add(point.AsGeoCoordinate());
            }

            return polyline;
        }

        public GeoCoordinate StartLocation
        {
            get
            {
                return this.route.RouteLegs[0].ActualStart.AsGeoCoordinate();
            }
        }

        public GeoCoordinate EndLocation
        {
            get
            {
                return this.route.RouteLegs[0].ActualEnd.AsGeoCoordinate();
            }
        }

        public double TravelDuration
        {
            get
            {
                return this.route.TravelDuration;
            }
        }

        public List<ItineraryStep> ItinerarySteps
        {
            get
            {
                return this.steps;
            }
        }

        public string ArrivalTime
        {
            get
            {
                return this.route.RouteLegs[0].EndTime.ToShortTimeString();
            }
        }

        public string DepartureTime
        {
            get
            {
                return this.route.RouteLegs[0].StartTime.ToShortTimeString();
            }
        }
    }

    public class ItineraryStep
    {
        ItineraryItem item;

        public ItineraryStep(ItineraryItem item)
        {
            this.item = item;
        }

        public GeoCoordinate GeoCoordinate
        {
            get
            {
                return this.item.ManeuverPoint.AsGeoCoordinate();
            }
        }

        public string Instruction
        {
            get
            {
                return this.item.Instruction.Value;
            }
        }

        public string TravelMode
        {
            get
            {
                return this.item.Detail.Mode == null ? "" : this.item.Detail.Mode;
            }
        }

        public string BusNumber
        {
            get
            {
                return this.item.TransitLine.AbbreviatedName;
            }
        }

        public string IconType
        {
            get
            {
                return this.item.IconType.ToString().StartsWith("N") ? "" : this.item.IconType.ToString();
            }
        }
    }

    public class LocationDescription
    {
        private Location bingMapsResult;
        private PhonebookResult bingSearchResult;

        public LocationDescription(Location result)
        {
            this.bingMapsResult = result;
        }

        public LocationDescription(PhonebookResult result)
        {
            this.bingSearchResult = result;
        }

        public string DisplayName
        {
            get
            {
                return this.bingMapsResult != null ?
                    this.bingMapsResult.Name :
                    this.bingSearchResult.Title;
            }
        }

        public GeoCoordinate GeoCoordinate
        {
            get
            {
                return this.bingMapsResult != null ?
                    this.bingMapsResult.Point.AsGeoCoordinate() :
                    new GeoCoordinate(this.bingSearchResult.Latitude, this.bingSearchResult.Longitude);
            }
        }

        public string PostalCode
        {
            get
            {
                return this.bingMapsResult != null ?
                   this.bingMapsResult.Address.PostalCode :
                   this.bingSearchResult.PostalCode;
            }
        }

        public string Address
        {
            get
            {
                return this.bingMapsResult != null ?
                    this.bingMapsResult.Address.FormattedAddress :
                    String.Format("{0} {1}, {2} {3}",
                        this.bingSearchResult.Address,
                        this.bingSearchResult.City,
                        this.bingSearchResult.StateOrProvince,
                        this.bingSearchResult.PostalCode);
            }
        }

        public string Confidence
        {
            get
            {
                return this.bingMapsResult != null ?
                    this.bingMapsResult.Confidence.ToString() :
                    "High";
            }
        }
    }
}
