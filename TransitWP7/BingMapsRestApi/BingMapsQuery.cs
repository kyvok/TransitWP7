﻿//TODO: copyright info

namespace TransitWP7.BingMapsRestApi
{
    using System;
    using System.Device.Location;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// Helper class to query BingMaps resources.
    /// </summary>
    public static class BingMapsQuery
    {
        private static OutputParameters DefaultOutputParameters = new OutputParameters(OutputFormat.Xml, suppressStatus: true);
        private static KeyParameter DefaultKeyParameter = new KeyParameter(BingMapsKey.Key);
        private static XmlSerializer BingMapsResponseSerializer = new XmlSerializer(typeof(Response));

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="point">Location on map.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        public static void GetLocationInfo(GeoCoordinate point, Action<BingMapsQueryResult> callback)
        {
            GetLocationInfo(point, callback, null);
        }

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="point">Location on map.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback</param>
        public static void GetLocationInfo(GeoCoordinate point, Action<BingMapsQueryResult> callback, object userState)
        {
            GetLocationsFromQuery(point.AsBingMapsPoint().ToString(), null, callback);
        }

        /// <summary>
        /// Takes a query string and query for possible locations.
        /// </summary>
        /// <param name="query">A query to submit.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        public static void GetLocationsFromQuery(string query, Action<BingMapsQueryResult> callback)
        {
            GetLocationsFromQuery(query, callback, null);
        }

        /// <summary>
        /// Takes a query string and query for possible locations.
        /// </summary>
        /// <param name="query">A query to submit.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback.</param>
        public static void GetLocationsFromQuery(string query, Action<BingMapsQueryResult> callback, object userState)
        {
            GetLocationsFromQuery(query, null, callback, userState);
        }

        /// <summary>
        /// Takes a query string and query for possible locations using the provided user context.
        /// </summary>
        /// <param name="query">A query to submit.</param>
        /// <param name="userContext">Information about the user context, like geographic coordinate and current map view port.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        public static void GetLocationsFromQuery(string query, UserContextParameters context, Action<BingMapsQueryResult> callback)
        {
            GetLocationsFromQuery(query, context, callback, null);
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
                "Locations/" + query,
                userContext != null ? userContext.ToString() : String.Empty);
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Takes a start and end points and query for possible transit routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        public static void GetTransitRoute(GeoCoordinate start, GeoCoordinate end, Action<BingMapsQueryResult> callback)
        {
            GetTransitRoute(start, end, callback, null);
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
            var queryUri = ConstructQueryUri(
                "Routes/Transit",
                new TransitQueryParameters(start.AsBingMapsPoint(), end.AsBingMapsPoint()) { MaxSolutions = 5 }.ToString());
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Takes a start and end points and query for possible transit routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="time">A time or date that relates to the route query.</param>
        /// <param name="timeType">The TimeType of the dateTime parameter.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        public static void GetTransitRoute(GeoCoordinate start, GeoCoordinate end, DateTime time, TimeType timeType, Action<BingMapsQueryResult> callback)
        {
            GetTransitRoute(start, end, time, timeType, callback, null);
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
            var queryUri = ConstructQueryUri(
                "Routes/Transit",
                new TransitQueryParameters(start.AsBingMapsPoint(), end.AsBingMapsPoint(), time, timeType) { MaxSolutions = 5 }.ToString());
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Takes a start and end points and query for possible walking routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        public static void GetWalkingRoute(GeoCoordinate start, GeoCoordinate end, Action<BingMapsQueryResult> callback)
        {
            GetWalkingRoute(start, end, callback, null);
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
                new RouteQueryParameters(start.AsBingMapsPoint(), end.AsBingMapsPoint()).ToString());
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
            const string BingMapsRESTServicesBaseAddress = "http://dev.virtualearth.net/REST/v1/";

            var uri = new StringBuilder();
            uri.Append(BingMapsRESTServicesBaseAddress);
            uri.Append(resourcePath);
            uri.Append("?");
            uri.Append(resourceQueryParameters);
            uri.Append(DefaultOutputParameters);
            uri.Append(DefaultKeyParameter);

            return new Uri(uri.ToString());
        }

        private static void ExecuteQuery(Uri queryUri, Action<BingMapsQueryResult> callback, object userState)
        {
            var httpRequest = WebRequest.Create(queryUri) as HttpWebRequest;
            var context = new BingMapsRequestContext(httpRequest, new BingMapsQueryAsyncCallback(callback, userState));
            httpRequest.BeginGetResponse(HttpRequestCompleted, context);
        }

        private static void HttpRequestCompleted(IAsyncResult asyncResult)
        {
            var context = asyncResult.AsyncState as BingMapsRequestContext;
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
                    StringBuilder exceptionMessage = new StringBuilder();
                    exceptionMessage.AppendLine("One or more error were returned by the query:");
                    foreach (string errorDetail in response.ErrorDetails)
                    {
                        exceptionMessage.Append("  ");
                        exceptionMessage.AppendLine(errorDetail);
                    }
                    context.AsyncCallback.Notify(new Exception(exceptionMessage.ToString()));
                }
                else
                {
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
            public HttpWebRequest HttpRequest { get; private set; }
            public BingMapsQueryAsyncCallback AsyncCallback { get; private set; }

            public BingMapsRequestContext(HttpWebRequest httpRequest, BingMapsQueryAsyncCallback callback)
            {
                this.HttpRequest = httpRequest;
                this.AsyncCallback = callback;
            }
        }
    }
}