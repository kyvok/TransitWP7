namespace GoogleApisLib.GooglePlacesApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Device.Location;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using Newtonsoft.Json;
    using SharpGIS;

    public enum AdultOption
    {
        Off,
        Moderate,
        Strict
    }

    public enum SearchOption
    {
        DisableLocationDetection,
        EnableHighlighting
    }

    public enum SourceType
    {
        Image,
        News,
        PhoneBook,
        RelatedSearch,
        Spell,
        Translation,
        Video,
        Web
    }

    public enum PhonebookSortOption
    {
        Default,
        Distance,
        Relevance
    }

    /// <summary>
    /// Helper class to query BingMaps resources.
    /// </summary>
    public static class GooglePlacesQuery
    {
        private static readonly ConcurrentDictionary<string, SearchResponse> GooglePlacesQueryInMemoryCache = new ConcurrentDictionary<string, SearchResponse>();

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="userLocation">Location on map.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback</param>
        public static void GetBusinessInfo(string query, GeoCoordinate userLocation, Action<GooglePlacesQueryResult> callback, object userState)
        {
            if (userLocation == null)
            {
                throw new ArgumentNullException("userLocation");
            }

            var request = new TextSearchRequest
            {
                ApiKey = ApiKeys.GooglePlacesKey,
                Query = Uri.EscapeDataString(query),
                Latitude = userLocation.Latitude,
                Longitude = userLocation.Longitude
            };
            var queryUri = ConstructQueryUri(request.ToString());
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Constructs the GooglePlaces REST URL
        /// </summary>
        /// <param name="resourceQueryParameters">The query parameters to apply to the resource.</param>
        /// <returns>The REST URL representing the resource query.</returns>
        private static Uri ConstructQueryUri(string resourceQueryParameters)
        {
            const string GooglePlacesRestServicesBaseAddress = "https://maps.googleapis.com/maps/api/place/textsearch/json";

            var uri = new StringBuilder();
            uri.Append(GooglePlacesRestServicesBaseAddress);
            uri.Append("?");
            uri.Append(resourceQueryParameters);

            return new Uri(uri.ToString());
        }

        private static void ExecuteQuery(Uri queryUri, Action<GooglePlacesQueryResult> callback, object userState)
        {
            if (GooglePlacesQueryInMemoryCache.ContainsKey(queryUri.ToString()))
            {
                callback(new GooglePlacesQueryResult(GooglePlacesQueryInMemoryCache[queryUri.ToString()], userState));
                return;
            }

            var httpRequest = WebRequestCreator.GZip.Create(queryUri);
            var context = new GooglePlacesRequestContext(httpRequest, new GooglePlacesQueryAsyncCallback(callback, userState));
            httpRequest.BeginGetResponse(HttpRequestCompleted, context);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Must provide exception back to the user thread.")]
        private static void HttpRequestCompleted(IAsyncResult asyncResult)
        {
            var context = (GooglePlacesRequestContext)asyncResult.AsyncState;
            if (context.AsyncCallback == null)
            {
                throw new InvalidOperationException("Unexpected exception, no GooglePlacesQueryAsyncCallback!");
            }

            try
            {
                var httpResponse = context.HttpRequest.EndGetResponse(asyncResult);
                string jsonString;
                using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    jsonString = reader.ReadToEnd();
                }

                var response = JsonConvert.DeserializeObject<SearchResponse>(jsonString);
                if (response.status != null && response.status != "OK")
                {
                    var exceptionMessage = new StringBuilder();
                    exceptionMessage.AppendLine("One or more error were returned by the query:");
                    foreach (var errorDetail in response.status)
                    {
                        exceptionMessage.Append("  ");
                        exceptionMessage.AppendLine(response.status);
                    }

                    context.AsyncCallback.Notify(new Exception(exceptionMessage.ToString()));
                }
                else
                {
                    GooglePlacesQueryInMemoryCache.TryAdd(context.HttpRequest.RequestUri.ToString(), response);
                    context.AsyncCallback.Notify(response);
                }
            }
            catch (Exception ex)
            {
                context.AsyncCallback.Notify(ex);
            }
        }

        private class GooglePlacesRequestContext
        {
            public GooglePlacesRequestContext(WebRequest httpRequest, GooglePlacesQueryAsyncCallback callback)
            {
                this.HttpRequest = httpRequest;
                this.AsyncCallback = callback;
            }

            public WebRequest HttpRequest { get; private set; }

            public GooglePlacesQueryAsyncCallback AsyncCallback { get; private set; }
        }
    }

    public class TextSearchRequest
    {
        // Required parameters

        // query string
        public string Query { get; set; }

        // Api key
        public string ApiKey { get; set; }

        // Location comes from sensor. true or false
        // we use true by default
        public bool LocFromSensor
        {
            get
            {
                return true;
            }
        }

        // Optional parameters

        // User location parameters
        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        // radius of the search to perform
        // maximum radius allowed is 50000 meters, which we use by default
        public int Radius
        {
            get
            {
                return 50000;
            }
        }

        public string Language
        {
            get
            {
                return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            }
        }

        public string[] Types { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("query=");
            builder.Append(this.Query);

            if (this.Latitude.HasValue && this.Longitude.HasValue)
            {
                builder.Append("&location=");
                builder.Append(this.Latitude.Value.ToString("G9", CultureInfo.InvariantCulture) + "," + this.Longitude.Value.ToString("G9", CultureInfo.InvariantCulture));
            }

            builder.Append("&radius=");
            builder.Append(this.Radius.ToString(CultureInfo.InvariantCulture));

            // TODO add support for types if needed
            // TODO add language token from UI when ready
            ////if (!string.IsNullOrWhiteSpace(this.Language))
            ////{
            ////    builder.Append("&language=");
            ////    builder.Append(this.Language);
            ////}

            builder.Append("&sensor=");
            builder.Append(this.LocFromSensor.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());

            builder.Append("&key=");
            builder.Append(this.ApiKey);

            return builder.ToString();
        }
    }
}
