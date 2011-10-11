//TODO: copyright info

namespace TransitWP7.BingMapsRestApi
{
    using System;
    using System.IO;
    using System.Net;
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// Helper class to query BingMaps resources.
    /// </summary>
    public static class BingMapsQuery
    {
        // TODO: consider extension methods to transform WP7 Location object to BingMaps.Point, WP7 MapViewPort to BingMaps.BoundingBox, etc

        private static OutputParameters DefaultOutputParameters = new OutputParameters(OutputFormat.Xml, suppressStatus: false);
        private static KeyParameter DefaultKeyParameter = new KeyParameter(BingMapsKey.Key);
        private static XmlSerializer BingMapsResponseSerializer = new XmlSerializer(typeof(Response));

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="point">Location on map</param>
        /// <param name="callback">Callback that will use the response result</param>
        public static void GetLocationInfo(Point point, Action<BingMapsQueryResult> callback)
        {
            GetLocationsFromQueryWithUserContext(point.ToString(), null, callback);
        }

        /// <summary>
        /// Takes a query string (address / business name / point / anything!) and query for possible locations.
        /// </summary>
        /// <param name="query">A query to submit</param>
        /// /// <param name="callback">Callback that will use the response result</param>
        public static void GetLocationsFromQuery(string query, Action<BingMapsQueryResult> callback)
        {
            GetLocationsFromQueryWithUserContext(query, null, callback);
        }

        /// <summary>
        /// Takes a query string (address / business name / point / anything!) and query for possible locations using the provided user context.
        /// </summary>
        /// <param name="query">A query to submit</param>
        /// <param name="userContext">Information about the user context, like geographic coordinate and current map view port.</param>
        /// <param name="callback">Callback that will use the response result</param>
        public static void GetLocationsFromQueryWithUserContext(string query, UserContextParameters userContext, Action<BingMapsQueryResult> callback)
        {
            Uri queryUri = ConstructQueryUri("Locations/" + query, userContext != null ? userContext.ToString() : String.Empty);
            ExecuteQuery<IEnumerable<Location>>(queryUri, callback);
        }

        public static void GetTransitRouteFromPoints(Point start, Point end, Action<BingMapsQueryResult> callback)
        {
            Uri queryUri = ConstructQueryUri("Routes/Transit", new TransitQueryParameters(start, end) { MaxSolutions = 5 }.ToString());
            ExecuteQuery<IEnumerable<Route>>(queryUri, callback);
        }

        public static void GetWalkingRouteFromPoints(Point start, Point end, Action<BingMapsQueryResult> callback)
        {
            Uri queryUri = ConstructQueryUri("Routes/Walking", new RouteQueryParameters(start, end).ToString());
            ExecuteQuery<IEnumerable<Route>>(queryUri, callback);
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

        //TODO: change later to use HttpWebRequest instead of WebClient, because WebClient marshalls back to UI thread in WP7.
        private static void ExecuteQuery<T>(Uri queryUri, Action<BingMapsQueryResult> callback)
        {
            var client = new WebClient();
            client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
            client.OpenReadAsync(queryUri, new BingMapsQueryAsyncCallback(callback));
        }

        private static void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.UserState is BingMapsQueryAsyncCallback)
            {
                var callback = e.UserState as BingMapsQueryAsyncCallback;

                if (e.Error != null)
                {
                    callback.Notify(e.Error);
                }

                Response response = (Response)BingMapsResponseSerializer.Deserialize(e.Result);

                //TODO: check status code for errors here, or consider not suppressing status code

                callback.Notify(response);
            }
            else
            {
                //TODO: this is bad, throw
            }
        }
    }
}
