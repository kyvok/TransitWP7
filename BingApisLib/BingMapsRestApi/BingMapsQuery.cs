namespace BingApisLib.BingMapsRestApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using SharpGIS;

    /// <summary>
    /// Helper class to query BingMaps resources.
    /// </summary>
    public static class BingMapsQuery
    {
        private static readonly OutputParameters DefaultOutputParameters = new OutputParameters(OutputFormat.Xml, suppressStatus: true);
        private static readonly KeyParameter DefaultKeyParameter = new KeyParameter(ApiKeys.BingMapsKey);
        private static readonly XmlSerializer BingMapsResponseSerializer = new XmlSerializer(typeof(Response));

        private static readonly ConcurrentDictionary<string, Response> BingMapsQueryInMemoryCache = new ConcurrentDictionary<string, Response>();

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="point">Location on map.</param>
        public static Task<BingMapsQueryResult> GetLocationInfo(GeoCoordinate point)
        {
            var queryUri = ConstructQueryUri(
                "Locations/" + point.AsBingMapsPoint(), null);
            return ExecuteQuery(queryUri);
        }

        /// <summary>
        /// Takes a query string and query for possible locations using the provided user context.
        /// </summary>
        /// <param name="query">A query to submit</param>
        /// <param name="userContext">Information about the user context, like geographic coordinate and current map view port.</param>
        public static Task<BingMapsQueryResult> GetLocationsFromQuery(string query, UserContextParameters userContext)
        {
            var queryUri = ConstructQueryUri(
                "Locations",
                "q=" + Uri.EscapeDataString(query) + (userContext != null ? userContext.ToString() : string.Empty));
            return ExecuteQuery(queryUri);
        }

        /// <summary>
        /// Takes a start and end points and query for possible transit routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        public static Task<BingMapsQueryResult> GetTransitRoute(GeoCoordinate start, GeoCoordinate end)
        {
            return GetTransitRoute(start, end, DateTime.Now, TimeType.Departure);
        }

        /// <summary>
        /// Takes a start and end points and query for possible transit routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="time">A time or date that relates to the route query.</param>
        /// <param name="timeType">The TimeType of the dateTime parameter.</param>
        public static Task<BingMapsQueryResult> GetTransitRoute(GeoCoordinate start, GeoCoordinate end, DateTime time, TimeType timeType)
        {
            string rqp = new TransitQueryParameters(start.AsBingMapsPoint(), end.AsBingMapsPoint(), time, timeType)
                             {
                                 MaxSolutions = 5,
                                 RoutePathOutput = RoutePathOutput.Points
                             }.ToString();
            var queryUri = ConstructQueryUri("Routes/Transit", rqp);
            return ExecuteQuery(queryUri);
        }

        /// <summary>
        /// Takes a start and end points and query for possible walking routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        public static Task<BingMapsQueryResult> GetWalkingRoute(GeoCoordinate start, GeoCoordinate end)
        {
            var queryUri = ConstructQueryUri(
                "Routes/Walking",
                new RouteQueryParameters(start.AsBingMapsPoint(), end.AsBingMapsPoint()) { RoutePathOutput = RoutePathOutput.Points, Tolerances = new List<double> { 0.0000005 } }.ToString());
            return ExecuteQuery(queryUri);
        }

        /// <summary>
        /// Constructs the BingMaps REST URL using the structure defined in http://msdn.microsoft.com/en-us/library/ff701720.aspx
        /// </summary>
        /// <param name="resourcePath">The REST resource to query.</param>
        /// <param name="resourceQueryParameters">The query parameters to apply to the resource.</param>
        /// <returns>The REST URL representing the resource query.</returns>
        private static Uri ConstructQueryUri(string resourcePath, string resourceQueryParameters)
        {
            const string BingMapsRestServicesBaseAddress = "http://dev.virtualearth.net/REST/v1/";

            var uri = new StringBuilder();
            uri.Append(BingMapsRestServicesBaseAddress);
            uri.Append(resourcePath);
            uri.Append("?");
            uri.Append(resourceQueryParameters);
            uri.Append(DefaultOutputParameters);
            uri.Append(DefaultKeyParameter);

            return new Uri(uri.ToString());
        }

        private static Task<BingMapsQueryResult> ExecuteQuery(Uri queryUri)
        {
            if (BingMapsQueryInMemoryCache.ContainsKey(queryUri.ToString()))
            {
                return Task.FromResult(new BingMapsQueryResult(BingMapsQueryInMemoryCache[queryUri.ToString()]));
            }

            var tf = new TaskFactory();
            var httpRequest = WebRequestCreator.GZip.Create(queryUri);
            return tf.FromAsync<WebResponse>(httpRequest.BeginGetResponse, httpRequest.EndGetResponse, null).ContinueWith(
                asyncResult =>
                {
                    try
                    {
                        var httpResponse = asyncResult.Result;
                        var response = (Response)BingMapsResponseSerializer.Deserialize(httpResponse.GetResponseStream());
                        if (response.ErrorDetails != null && response.ErrorDetails.Length > 0)
                        {
                            var exceptionMessage = new StringBuilder();
                            exceptionMessage.AppendLine("One or more error were returned by the query:");
                            foreach (var errorDetail in response.ErrorDetails)
                            {
                                exceptionMessage.Append("  ");
                                exceptionMessage.AppendLine(errorDetail);
                            }

                            return new BingMapsQueryResult(new Exception(exceptionMessage.ToString()));
                        }

                        BingMapsQueryInMemoryCache.TryAdd(queryUri.ToString(), response);
                        return new BingMapsQueryResult(response);
                    }
                    catch (Exception ex)
                    {
                        return new BingMapsQueryResult(ex);
                    }
                });
        }
    }
}
