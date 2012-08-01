namespace GoogleApisLib.GooglePlacesApi
{
    using System;

    public class GooglePlacesQueryResult
    {
        public GooglePlacesQueryResult(SearchResponse response)
            : this(response, null, null)
        {
        }

        public GooglePlacesQueryResult(Exception exception)
            : this(null, exception, null)
        {
        }

        public GooglePlacesQueryResult(SearchResponse response, object userState)
            : this(response, null, userState)
        {
        }

        public GooglePlacesQueryResult(Exception exception, object userState)
            : this(null, exception, userState)
        {
        }

        private GooglePlacesQueryResult(SearchResponse response, Exception exception, object userState)
        {
            this.Response = response;
            this.Error = exception;
            this.UserState = userState;
        }

        /// <summary>
        /// Gets a value indicating which exception occured if any.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the Bing Maps Response object returned by this query.
        /// </summary>
        public SearchResponse Response { get; private set; }

        /// <summary>
        /// Gets the user state associated with this query.
        /// </summary>
        public object UserState { get; private set; }
    }
}
