// TODO: walk does not need start and stop time
namespace TransitWP7.Model
{
    using System;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Linq;
    using System.Threading.Tasks;
    using BingApisLib.BingMapsRestApi;
    using GoogleApisLib.GoogleMapsApi;

    /// <summary>
    /// Calls REST APIs and isolate their type mapping by converting to transitive types.
    /// </summary>
    public static class ProxyQuery
    {
        public static Task<IEnumerable<LocationDescription>> GetLocationAddress(GeoCoordinate locationToQuery)
        {
            return BingMapsQuery.GetLocationInfo(locationToQuery).ContinueWith(
                continuation =>
                {
                    var result = continuation.Result;

                    if (result.Error == null)
                    {
                        return result.Response.GetLocations().Select(location => new LocationDescription(location));
                    }

                    return new List<LocationDescription>();
                });
        }

        public static Task<IEnumerable<LocationDescription>> GetLocationsAndBusiness(string query, GeoCoordinate currentUserPosition)
        {
            var locations = new List<LocationDescription>();

            return GetBingLocations(query, currentUserPosition).ContinueWith(
                bingContinuation =>
                {
                    return GetGooglePlaces(query, currentUserPosition).ContinueWith(
                        googleContinuation =>
                        {
                            if (bingContinuation.IsCompleted)
                            {
                                locations.AddRange(bingContinuation.Result);
                            }

                            if (googleContinuation.IsCompleted)
                            {
                                locations.AddRange(googleContinuation.Result);
                            }

                            locations = SortLocationDescriptionsByDistance(locations, currentUserPosition);

                            return locations.AsEnumerable();
                        });
                }).Unwrap();
        }

        public static Task<IEnumerable<TransitDescription>> GetTransitDirections(GeoCoordinate startPoint, GeoCoordinate endPoint, DateTime dateTime, TimeCondition timeType)
        {
            var transitDescriptions = new List<TransitDescription>();

            return GetBingTransitDirections(startPoint, endPoint, dateTime, timeType).ContinueWith(
                googleContinuation =>
                {
                    return GetBingWalkingDirections(startPoint, endPoint).ContinueWith(
                        bingContinuation =>
                        {
                            if (googleContinuation.IsCompleted)
                            {
                                transitDescriptions.AddRange(googleContinuation.Result);
                            }

                            if (bingContinuation.IsCompleted)
                            {
                                transitDescriptions.AddRange(bingContinuation.Result);
                            }

                            return transitDescriptions.AsEnumerable();
                        });
                }).Unwrap();
        }

        private static Task<IEnumerable<LocationDescription>> GetBingLocations(string query, GeoCoordinate currentUserPosition)
        {
            return BingMapsQuery.GetLocationsFromQuery(
                query, new UserContextParameters(currentUserPosition))
                .ContinueWith(
                    continuation =>
                    {
                        var result = continuation.Result;

                        if (result.Error == null)
                        {
                            return result.Response.GetLocations().Select(location => new LocationDescription(location));
                        }

                        return new List<LocationDescription>();
                    });
        }

        private static Task<IEnumerable<LocationDescription>> GetGooglePlaces(string query, GeoCoordinate currentUserPosition)
        {
            return GoogleApisLib.GooglePlacesApi.GooglePlacesQuery.GetBusinessInfo(
                query,
                currentUserPosition).ContinueWith(continuation =>
                    {
                        var result = continuation.Result;

                        if (result.Error == null && result.Response.results != null)
                        {
                            return result.Response.results.Select(location => new LocationDescription(location));
                        }

                        return new List<LocationDescription>();
                    });
        }

        private static Task<IEnumerable<TransitDescription>> GetBingTransitDirections(GeoCoordinate startPoint, GeoCoordinate endPoint, DateTime dateTime, TimeCondition timeType)
        {
            timeType = timeType == TimeCondition.Now ? TimeCondition.DepartingAt : timeType;
            return BingMapsQuery.GetTransitRoute(startPoint, endPoint, dateTime, (TimeType)timeType).ContinueWith(
                continuation =>
                {
                    var result = continuation.Result;
                    if (result.Error == null)
                    {
                        return result.Response.GetRoutes().Select(route => new TransitDescription(route, TransitDescription.DirectionType.Transit));
                    }

                    return new List<TransitDescription>();
                });
        }

        private static Task<IEnumerable<TransitDescription>> GetGoogleTransitDirections(GeoCoordinate startPoint, GeoCoordinate endPoint, DateTime dateTime, TimeCondition timeType)
        {
            bool isDepartureTime = timeType == TimeCondition.Now || timeType == TimeCondition.DepartingAt;
            return GoogleMapsQuery.GetTransitRoute(startPoint, endPoint, dateTime, isDepartureTime).ContinueWith(
                continuation =>
                {
                    var result = continuation.Result;
                    if (result.Error == null)
                    {
                        return result.Response.routes.Select(route => new TransitDescription(route, TransitDescription.DirectionType.Transit));
                    }

                    return new List<TransitDescription>();
                });
        }

        private static Task<IEnumerable<TransitDescription>> GetBingWalkingDirections(GeoCoordinate startPoint, GeoCoordinate endPoint)
        {
            if (startPoint.GetDistanceTo(endPoint) > 6000)
            {
                return Task.FromResult(new List<TransitDescription>().AsEnumerable());
            }

            return BingMapsQuery.GetWalkingRoute(startPoint, endPoint).ContinueWith(
                continuation =>
                {
                    var result = continuation.Result;
                    if (result.Error == null)
                    {
                        return result.Response.GetRoutes().Select(route => new TransitDescription(route, TransitDescription.DirectionType.Transit));
                    }

                    return new List<TransitDescription>();
                });
        }

        private static List<LocationDescription> SortLocationDescriptionsByDistance(IEnumerable<LocationDescription> locations, GeoCoordinate center)
        {
            // Using 80 miles since Phonebook is 80 miles and that 99% case is a local search for transit.
            // http://msdn.microsoft.com/en-us/library/dd250980.aspx
            // 80 miles * 1.6 miles per km * 1000 m per km
            const double MaxRange = 80 * 1.6 * 1000;

            if (locations == null)
            {
                return null;
            }

            var locs = locations.ToDictionary(k => k, v => v.GeoCoordinate.GetDistanceTo(center));

            // orderby is a stable sort
            // the result preserves the original sort (likely relevance), but moves item more then 80 miles to the bottom
            var sorted = locs
                        .Where(kvp => kvp.Value <= MaxRange)
                        .Select(items => items.Key)
                        .ToList();

            var outsideOfRange = locs
                                .Where(kvp => kvp.Value > MaxRange)
                                .Select(items => items.Key)
                                .ToList();

            sorted.AddRange(outsideOfRange);
            return sorted;
        }
    }
}
