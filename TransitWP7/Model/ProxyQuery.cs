using System;
using System.Collections.Generic;
using System.Device.Location;
using BingApisLib.BingMapsRestApi;
using BingApisLib.BingSearchRestApi;

// TODO: show total travel time for trips
// TODO: walk does not need start and stop time
namespace TransitWP7
{
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

            // chain querying for business
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
                foreach (var phonebookResult in result.Response.Phonebook.Results)
                {
                    if (queryState.LocationDescriptions == null)
                    {
                        queryState.LocationDescriptions = new List<LocationDescription>();
                    }

                    queryState.LocationDescriptions.Add(new LocationDescription(phonebookResult));
                }
            }
            else
            {
                queryState.SavedException = result.Error;
            }

            // call user callback
            var proxyQueryResult = new ProxyQueryResult() { UserState = queryState.UserState };
            if (queryState.LocationDescriptions != null && queryState.LocationDescriptions.Count > 0)
            {
                proxyQueryResult.LocationDescriptions = queryState.LocationDescriptions;
            }
            else
            {
                // TODO: probably try to do spelling correction here.
                proxyQueryResult.Error = new Exception(string.Format("Could not locate a result for {0} within 80 miles of your location.", queryState.Query));
                ////if (queryState.SavedException != null)
                ////{
                ////    proxyQueryResult.Error = queryState.SavedException;
                ////}
                ////else
                ////{
                ////    proxyQueryResult.Error = new Exception("no results");
                ////}
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

            // chain immediately to get walking directions
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

            // call user callback
            var proxyQueryResult = new ProxyQueryResult() { UserState = queryState.UserState };
            if (queryState.TransitDescriptions != null && queryState.TransitDescriptions.Count > 0)
            {
                proxyQueryResult.TransitDescriptions = queryState.TransitDescriptions;
            }
            else
            {
                proxyQueryResult.Error = new Exception("No transit or less then 90 minutes walk could be found.");
                ////if (queryState.SavedException != null)
                ////{
                ////    proxyQueryResult.Error = queryState.SavedException;
                ////}
                ////else
                ////{
                ////    proxyQueryResult.Error = new Exception("no results");
                ////}
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
}
