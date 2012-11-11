namespace GoogleApisLib.GoogleMapsApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Device.Location;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using SharpGIS;
    using System.Globalization;

    /// <summary>
    /// Helper class to query GoogleMaps resources.
    /// </summary>
    public static class GoogleMapsQuery
    {
        private static readonly ConcurrentDictionary<string, DirectionsResult> GoogleMapsQueryInMemoryCache = new ConcurrentDictionary<string, DirectionsResult>();

        /// <summary>
        /// Takes a start and end points and query for possible transit routes.
        /// </summary>
        /// <param name="start">Start location.</param>
        /// <param name="end">End location.</param>
        /// <param name="time">A time or date that relates to the route query.</param>
        /// <param name="isDepartCondition">The TimeType of the dateTime parameter.</param>
        public static Task<GoogleMapsQueryResult> GetTransitRoute(GeoCoordinate start, GeoCoordinate end, DateTime time, bool isDepartCondition)
        {
            const string GoogleMapsRestServicesBaseAddress = "https://maps.googleapis.com/maps/api/directions/json";
            var midnight1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var secondsSince1970 = time.ToUniversalTime() - midnight1970;

            var query = new StringBuilder(256);
            query.Append(GoogleMapsRestServicesBaseAddress);
            query.Append(string.Format(
                    "?origin={0},{1}",
                    start.Latitude.ToString("F6", NumberFormatInfo.InvariantInfo),
                    start.Longitude.ToString("F6", NumberFormatInfo.InvariantInfo)));
            query.Append(string.Format(
                    "&destination={0},{1}",
                    end.Latitude.ToString("F6", NumberFormatInfo.InvariantInfo),
                    end.Longitude.ToString("F6", NumberFormatInfo.InvariantInfo)));
            query.Append("&mode=transit");
            query.Append("&alternatives=true");
            query.Append((isDepartCondition ? "&departure_time=" : "&arrival_time=") + ((long)secondsSince1970.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
            query.Append("&units=metric");
            query.Append("&sensor=false");

            var queryUri = new Uri(query.ToString());
            return ExecuteQuery(queryUri);
        }

        private static Task<GoogleMapsQueryResult> ExecuteQuery(Uri queryUri)
        {
            if (GoogleMapsQueryInMemoryCache.ContainsKey(queryUri.ToString()))
            {
                return Task.FromResult(new GoogleMapsQueryResult(GoogleMapsQueryInMemoryCache[queryUri.ToString()]));
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

                        var response = JsonConvert.DeserializeObject<DirectionsResult>(jsonString);
                        if (response.status != null && response.status != "OK")
                        {
                            return new GoogleMapsQueryResult(new Exception(string.Format("One or more error were returned by the query:  {0}", response.status)));
                        }

                        GoogleMapsQueryInMemoryCache.TryAdd(httpRequest.RequestUri.ToString(), response);
                        return new GoogleMapsQueryResult(response);
                    }
                    catch (Exception ex)
                    {
                        return new GoogleMapsQueryResult(ex);
                    }
                });
        }
    }
}
