namespace GoogleApisLib.GooglePlacesApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Device.Location;
    using System.Globalization;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SharpGIS;

    /// <summary>
    /// Helper class to query GooglePlaces resources.
    /// </summary>
    public static class GooglePlacesQuery
    {
        private static readonly ConcurrentDictionary<string, SearchResponse> GooglePlacesQueryInMemoryCache = new ConcurrentDictionary<string, SearchResponse>();

        /// <summary>
        /// Takes a latitude/longitude location and query for the information related to this location.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="userLocation">Location on map.</param>
        public static Task<GooglePlacesQueryResult> GetBusinessInfo(string query, GeoCoordinate userLocation)
        {
            const string GooglePlacesRestServicesBaseAddress = "https://maps.googleapis.com/maps/api/place/textsearch/json";

            if (userLocation == null)
            {
                throw new ArgumentNullException("userLocation");
            }

            var builder = new StringBuilder();
            builder.Append(GooglePlacesRestServicesBaseAddress);
            builder.Append("?");
            builder.Append("query=" + Uri.EscapeDataString(query));

            if (userLocation != null)
            {
                builder.Append(string.Format(
                    "&location={0},{1}",
                    userLocation.Latitude.ToString("F6", NumberFormatInfo.InvariantInfo),
                    userLocation.Longitude.ToString("F6", NumberFormatInfo.InvariantInfo)));
            }

            builder.Append("&radius=50000");

            // TODO add support for types if needed
            // TODO add language token from UI when ready
            ////if (!string.IsNullOrWhiteSpace(this.Language))
            ////{
            ////    builder.Append("&language=");
            ////    builder.Append(this.Language);
            ////}

            builder.Append("&sensor=true");
            builder.Append("&key=" + ApiKeys.GooglePlacesKey);

            var queryUri = new Uri(builder.ToString());
            return ExecuteQuery(queryUri);
        }

        private static Task<GooglePlacesQueryResult> ExecuteQuery(Uri queryUri)
        {
            if (GooglePlacesQueryInMemoryCache.ContainsKey(queryUri.ToString()))
            {
                return Task.FromResult(new GooglePlacesQueryResult(GooglePlacesQueryInMemoryCache[queryUri.ToString()]));
            }

            var tf = new TaskFactory();
            var httpRequest = WebRequestCreator.GZip.Create(queryUri);
            return tf.FromAsync<WebResponse>(httpRequest.BeginGetResponse, httpRequest.EndGetResponse, null).ContinueWith(
                asyncResult =>
                {
                    try
                    {
                        var httpResponse = asyncResult.Result;
                        string jsonString;
                        using (var reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                        {
                            jsonString = reader.ReadToEnd();
                        }

                        var response = JsonConvert.DeserializeObject<SearchResponse>(jsonString);
                        if (response.status != null && response.status != "OK")
                        {
                            return new GooglePlacesQueryResult(new Exception(string.Format("One or more error were returned by the query:  {0}", response.status)));
                        }

                        GooglePlacesQueryInMemoryCache.TryAdd(httpRequest.RequestUri.ToString(), response);
                        return new GooglePlacesQueryResult(response);
                    }
                    catch (Exception ex)
                    {
                        return new GooglePlacesQueryResult(ex);
                    }
                });
        }
    }
}
