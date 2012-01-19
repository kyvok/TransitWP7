//TODO: copyright info

namespace BingApisLib.BingSearchRestApi
{
    using System;
    using System.Device.Location;
    using System.Net;
    using System.Text;
    using System.Xml.Serialization;
    using Newtonsoft.Json;

    /// <summary>
    /// Helper class to query BingMaps resources.
    /// </summary>
    public static class BingSearchQuery
    {
        private static XmlSerializer BingSearchResponseSerializer = new XmlSerializer(typeof(SearchResponse));

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="point">Location on map.</param>
        /// <param name="callback">Callback that will use the response result.</param>
        /// <param name="userState">An object to pass to the callback</param>
        public static void GetBusinessInfo(string query, GeoCoordinate userLocation, Action<BingSearchQueryResult> callback, object userState)
        {
            var request = new PhonebookRequest()
            {
                Query = query,
                Latitude = userLocation.Latitude,
                Longitude = userLocation.Longitude,
                Radius = 80,
                Count = 20,
                SortBy = PhonebookSortOption.Distance,
                Sources = new SourceType[] { SourceType.PhoneBook },
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
            var request = new SpellRequest()
            {
                Query = query,
                Sources = new SourceType[] { SourceType.Spell },
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
            const string BingSearchRESTServicesBaseAddress = "http://api.bing.net/json.aspx";

            var uri = new StringBuilder();
            uri.Append(BingSearchRESTServicesBaseAddress);
            uri.Append("?");
            uri.Append(resourceQueryParameters);

            return new Uri(uri.ToString());
        }

        private static void ExecuteQuery(Uri queryUri, Action<BingSearchQueryResult> callback, object userState)
        {
            var httpRequest = WebRequest.Create(queryUri) as HttpWebRequest;
            var context = new BingSearchRequestContext(httpRequest, new BingSearchQueryAsyncCallback(callback, userState));
            httpRequest.BeginGetResponse(HttpRequestCompleted, context);
        }

        private static void HttpRequestCompleted(IAsyncResult asyncResult)
        {
            var context = asyncResult.AsyncState as BingSearchRequestContext;
            if (context.AsyncCallback == null)
            {
                throw new InvalidOperationException("Unexpected exception, no BingMapsQueryAsyncCallback!");
            }

            try
            {
                var httpResponse = context.HttpRequest.EndGetResponse(asyncResult);
                //var response = (SearchResponse)BingSearchResponseSerializer.Deserialize(httpResponse.GetResponseStream());
                string jsonString = String.Empty;
                using (System.IO.StreamReader reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    jsonString = reader.ReadToEnd();
                }
                var wrapper = JsonConvert.DeserializeObject<BingSearchWrapper>(jsonString);
                var response = wrapper.SearchResponse;
                if (response.Errors != null && response.Errors.Length > 0)
                {
                    StringBuilder exceptionMessage = new StringBuilder();
                    exceptionMessage.AppendLine("One or more error were returned by the query:");
                    foreach (Error errorDetail in response.Errors)
                    {
                        exceptionMessage.Append("  ");
                        exceptionMessage.AppendLine(errorDetail.Message);
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

        private class BingSearchRequestContext
        {
            public HttpWebRequest HttpRequest { get; private set; }
            public BingSearchQueryAsyncCallback AsyncCallback { get; private set; }

            public BingSearchRequestContext(HttpWebRequest httpRequest, BingSearchQueryAsyncCallback callback)
            {
                this.HttpRequest = httpRequest;
                this.AsyncCallback = callback;
            }
        }
    }

    public class BingSearchWrapper
    {
        public SearchResponse SearchResponse { get; set; }
    }

    public class SearchRequest
    {
        //required
        public string AppId { get; set; }
        public string Query { get; set; }
        public SourceType[] Sources { get; set; }

        //optional
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
            //TODO: This can be multiple sources
            builder.Append(this.Sources[0].ToString());
            if (!String.IsNullOrWhiteSpace(this.Version))
            {
                builder.Append("&Version=");
                builder.Append(this.Version);
            }
            if (!String.IsNullOrWhiteSpace(this.Market))
            {
                builder.Append("&Market=");
                builder.Append(this.Market);
            }
            //TODO: insert AdultOption
            if (!String.IsNullOrWhiteSpace(this.UILanguage))
            {
                builder.Append("&UILanguage=");
                builder.Append(this.UILanguage);
            }
            if (this.Latitude.HasValue)
            {
                builder.Append("&Latitude=");
                builder.Append(this.Latitude.Value.ToString("G9"));
            }
            if (this.Longitude.HasValue)
            {
                builder.Append("&Longitude=");
                builder.Append(this.Longitude.Value.ToString("G9"));
            }
            if (this.Radius.HasValue)
            {
                builder.Append("&Radius=");
                builder.Append(this.Radius.Value.ToString("G9"));
            }
            //TODO: insert SearchOption

            return builder.ToString();
        }
    }

    public class PhonebookRequest : SearchRequest
    {
        //optional
        public uint? Count { get; set; } //max 25, min 1, default 10
        public uint? Offset { get; set; } // count+offset should not go over 1000
        public string FileType { get; set; } //do not use
        public PhonebookSortOption? SortBy { get; set; }
        //TODO: do we want this LocID parameter?
        //public string LocID { get; set; } //lookup a specific phonebook entry by id.

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
                builder.Append(this.SortBy.ToString());
            }
            //TODO: insert FileType

            return base.ToString() + builder.ToString();
        }
    }

    public class SpellRequest : SearchRequest
    {
    }

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
}
