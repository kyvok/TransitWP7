namespace BingApisLib.BingSearchRestApi
{
    using System;

    public class BingSearchQueryResult
    {
        public BingSearchQueryResult(SearchResponse response)
            : this(response, null, null)
        {
        }

        public BingSearchQueryResult(Exception exception)
            : this(null, exception, null)
        {
        }

        public BingSearchQueryResult(SearchResponse response, object userState)
            : this(response, null, userState)
        {
        }

        public BingSearchQueryResult(Exception exception, object userState)
            : this(null, exception, userState)
        {
        }

        private BingSearchQueryResult(SearchResponse response, Exception exception, object userState)
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
