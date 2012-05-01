namespace BingApisLib.BingMapsRestApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Device.Location;
    using System.Net;
    using System.Text;
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
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback</param>
        public static void GetLocationInfo(GeoCoordinate point, Action<BingMapsQueryResult> callback, object userState)
        {
            var queryUri = ConstructQueryUri(
                "Locations/" + point.AsBingMapsPoint(), null);
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Takes a query string and query for possible locations using the provided user context.
        /// </summary>
        /// <param name="query">A query to submit</param>
        /// <param name="userContext">Information about the user context, like geographic coordinate and current map view port.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback.</param>
        public static void GetLocationsFromQuery(string query, UserContextParameters userContext, Action<BingMapsQueryResult> callback, object userState)
        {
            var queryUri = ConstructQueryUri(
                "Locations",
                "q=" + Uri.EscapeDataString(query) + (userContext != null ? userContext.ToString() : string.Empty));
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Takes a start and end points and query for possible transit routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback.</param>
        public static void GetTransitRoute(GeoCoordinate start, GeoCoordinate end, Action<BingMapsQueryResult> callback, object userState)
        {
            GetTransitRoute(start, end, DateTime.Now, TimeType.Departure, callback, userState);
        }

        /// <summary>
        /// Takes a start and end points and query for possible transit routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="time">A time or date that relates to the route query.</param>
        /// <param name="timeType">The TimeType of the dateTime parameter.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback.</param>
        public static void GetTransitRoute(GeoCoordinate start, GeoCoordinate end, DateTime time, TimeType timeType, Action<BingMapsQueryResult> callback, object userState)
        {
            string rqp = new TransitQueryParameters(start.AsBingMapsPoint(), end.AsBingMapsPoint(), time, timeType)
                             {
                                 MaxSolutions = 5,
                                 RoutePathOutput = RoutePathOutput.Points
                             }.ToString();
            var queryUri = ConstructQueryUri("Routes/Transit", rqp);
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Takes a start and end points and query for possible walking routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback.</param>
        public static void GetWalkingRoute(GeoCoordinate start, GeoCoordinate end, Action<BingMapsQueryResult> callback, object userState)
        {
            var queryUri = ConstructQueryUri(
                "Routes/Walking",
                new RouteQueryParameters(start.AsBingMapsPoint(), end.AsBingMapsPoint()) { RoutePathOutput = RoutePathOutput.Points, Tolerances = new System.Collections.Generic.List<double> { 0.0000005 } }.ToString());
            ExecuteQuery(queryUri, callback, userState);
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

        private static void ExecuteQuery(Uri queryUri, Action<BingMapsQueryResult> callback, object userState)
        {
            if (BingMapsQueryInMemoryCache.ContainsKey(queryUri.ToString()))
            {
                callback(new BingMapsQueryResult(BingMapsQueryInMemoryCache[queryUri.ToString()], userState));
                return;
            }

            var httpRequest = WebRequestCreator.GZip.Create(queryUri);
            var context = new BingMapsRequestContext(httpRequest, new BingMapsQueryAsyncCallback(callback, userState));
            httpRequest.BeginGetResponse(HttpRequestCompleted, context);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Must provide exception back to the user thread.")]
        private static void HttpRequestCompleted(IAsyncResult asyncResult)
        {
            var context = (BingMapsRequestContext)asyncResult.AsyncState;
            if (context.AsyncCallback == null)
            {
                throw new InvalidOperationException("Unexpected exception, no BingMapsQueryAsyncCallback!");
            }

            try
            {
                var httpResponse = context.HttpRequest.EndGetResponse(asyncResult);
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

                    context.AsyncCallback.Notify(new Exception(exceptionMessage.ToString()));
                }
                else
                {
                    BingMapsQueryInMemoryCache.TryAdd(context.HttpRequest.RequestUri.ToString(), response);
                    context.AsyncCallback.Notify(response);
                }
            }
            catch (Exception ex)
            {
                context.AsyncCallback.Notify(ex);
            }
        }

        private class BingMapsRequestContext
        {
            public BingMapsRequestContext(WebRequest httpRequest, BingMapsQueryAsyncCallback callback)
            {
                this.HttpRequest = httpRequest;
                this.AsyncCallback = callback;
            }

            public WebRequest HttpRequest { get; private set; }

            public BingMapsQueryAsyncCallback AsyncCallback { get; private set; }
        }
    }
}
