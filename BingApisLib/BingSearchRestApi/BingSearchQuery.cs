namespace BingApisLib.BingSearchRestApi
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
    public static class BingSearchQuery
    {
        private static readonly ConcurrentDictionary<string, SearchResponse> BingSearchQueryInMemoryCache = new ConcurrentDictionary<string, SearchResponse>();

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="userLocation">Location on map.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback</param>
        public static void GetBusinessInfo(string query, GeoCoordinate userLocation, Action<BingSearchQueryResult> callback, object userState)
        {
            if (userLocation == null)
            {
                throw new ArgumentNullException("userLocation");
            }

            var request = new PhonebookRequest
            {
                Query = Uri.EscapeDataString(query),
                Latitude = userLocation.Latitude,
                Longitude = userLocation.Longitude,
                Radius = 80,
                Count = 25,
                SortBy = PhonebookSortOption.Relevance,
                Sources = new[] { SourceType.PhoneBook },
                AppId = ApiKeys.BingSearchKey
            };
            var queryUri = ConstructQueryUri(request.ToString());
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Takes a query and returns suggested spelling for that entry.
        /// </summary>
        /// <param name="query">Query to suggest spelling for.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback</param>
        public static void GetSpellSuggestions(string query, Action<BingSearchQueryResult> callback, object userState)
        {
            var request = new SpellRequest
            {
                Query = Uri.EscapeDataString(query),
                Sources = new[] { SourceType.Spell },
                AppId = ApiKeys.BingSearchKey
            };
            var queryUri = ConstructQueryUri(request.ToString());
            ExecuteQuery(queryUri, callback, userState);
        }

        /// <summary>
        /// Constructs the BingSearch REST URL
        /// </summary>
        /// <param name="resourceQueryParameters">The query parameters to apply to the resource.</param>
        /// <returns>The REST URL representing the resource query.</returns>
        private static Uri ConstructQueryUri(string resourceQueryParameters)
        {
            const string BingSearchRestServicesBaseAddress = "http://api.bing.net/json.aspx";

            var uri = new StringBuilder();
            uri.Append(BingSearchRestServicesBaseAddress);
            uri.Append("?");
            uri.Append(resourceQueryParameters);

            return new Uri(uri.ToString());
        }

        private static void ExecuteQuery(Uri queryUri, Action<BingSearchQueryResult> callback, object userState)
        {
            if (BingSearchQueryInMemoryCache.ContainsKey(queryUri.ToString()))
            {
                callback(new BingSearchQueryResult(BingSearchQueryInMemoryCache[queryUri.ToString()], userState));
                return;
            }

            var httpRequest = WebRequestCreator.GZip.Create(queryUri);
            var context = new BingSearchRequestContext(httpRequest, new BingSearchQueryAsyncCallback(callback, userState));
            httpRequest.BeginGetResponse(HttpRequestCompleted, context);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Must provide exception back to the user thread.")]
        private static void HttpRequestCompleted(IAsyncResult asyncResult)
        {
            var context = (BingSearchRequestContext)asyncResult.AsyncState;
            if (context.AsyncCallback == null)
            {
                throw new InvalidOperationException("Unexpected exception, no BingMapsQueryAsyncCallback!");
            }

            try
            {
                var httpResponse = context.HttpRequest.EndGetResponse(asyncResult);
                string jsonString;
                using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    jsonString = reader.ReadToEnd();
                }

                var wrapper = JsonConvert.DeserializeObject<BingSearchWrapper>(jsonString);
                var response = wrapper.SearchResponse;
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    var exceptionMessage = new StringBuilder();
                    exceptionMessage.AppendLine("One or more error were returned by the query:");
                    foreach (var errorDetail in response.Errors)
                    {
                        exceptionMessage.Append("  ");
                        exceptionMessage.AppendLine(errorDetail.Message);
                    }

                    context.AsyncCallback.Notify(new Exception(exceptionMessage.ToString()));
                }
                else
                {
                    BingSearchQueryInMemoryCache.TryAdd(context.HttpRequest.RequestUri.ToString(), response);
                    context.AsyncCallback.Notify(response);
                }
            }
            catch (Exception ex)
            {
                context.AsyncCallback.Notify(ex);
            }
        }

        private class BingSearchRequestContext
        {
            public BingSearchRequestContext(WebRequest httpRequest, BingSearchQueryAsyncCallback callback)
            {
                this.HttpRequest = httpRequest;
                this.AsyncCallback = callback;
            }

            public WebRequest HttpRequest { get; private set; }

            public BingSearchQueryAsyncCallback AsyncCallback { get; private set; }
        }
    }

    public class BingSearchWrapper
    {
        public SearchResponse SearchResponse { get; set; }
    }

    public class SearchRequest
    {
        // required
        public string AppId { get; set; }

        public string Query { get; set; }

        public SourceType[] Sources { get; set; }

        // optional
        public string Version { get; set; }

        public string Market { get; set; }

        public AdultOption? Adult { get; set; }

        public string UILanguage { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? Radius { get; set; }

        public SearchOption[] Options { get; set; }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.Append("&AppId=");
            builder.Append(this.AppId);
            builder.Append("&Query=");
            builder.Append(this.Query);
            builder.Append("&Sources=");

            // TODO: This can be multiple sources
            builder.Append(this.Sources[0].ToString());
            if (!string.IsNullOrWhiteSpace(this.Version))
            {
                builder.Append("&Version=");
                builder.Append(this.Version);
            }

            if (!string.IsNullOrWhiteSpace(this.Market))
            {
                builder.Append("&Market=");
                builder.Append(this.Market);
            }

            // TODO: insert AdultOption
            if (!string.IsNullOrWhiteSpace(this.UILanguage))
            {
                builder.Append("&UILanguage=");
                builder.Append(this.UILanguage);
            }

            if (this.Latitude.HasValue)
            {
                builder.Append("&Latitude=");
                builder.Append(this.Latitude.Value.ToString("G9", CultureInfo.InvariantCulture));
            }

            if (this.Longitude.HasValue)
            {
                builder.Append("&Longitude=");
                builder.Append(this.Longitude.Value.ToString("G9", CultureInfo.InvariantCulture));
            }

            if (this.Radius.HasValue)
            {
                builder.Append("&Radius=");
                builder.Append(this.Radius.Value.ToString("G9", CultureInfo.InvariantCulture));
            }

            // TODO: insert SearchOption
            return builder.ToString();
        }
    }

    public class PhonebookRequest : SearchRequest
    {
        // optional
        // max 25, min 1, default 10
        public uint? Count { get; set; }

        // count+offset should not go over 1000
        public uint? Offset { get; set; }

        // do not use
        public string FileType { get; set; }

        public PhonebookSortOption? SortBy { get; set; }

        // TODO: do we want this LocID parameter?
        ////public string LocID { get; set; } //lookup a specific phonebook entry by id.

        public override string ToString()
        {
            var builder = new StringBuilder();
            if (this.Count.HasValue)
            {
                builder.Append("&Phonebook.Count=");
                builder.Append(this.Count);
            }

            if (this.Offset.HasValue)
            {
                builder.Append("&Phonebook.Offset=");
                builder.Append(this.Offset);
            }

            if (this.SortBy.HasValue)
            {
                builder.Append("&Phonebook.SortBy=");
                builder.Append(this.SortBy.Value.ToString());
            }

            // TODO: insert FileType
            return base.ToString() + builder;
        }
    }

    public class SpellRequest : SearchRequest
    {
    }
}
