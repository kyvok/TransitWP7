//TODO: copyright info

namespace BingApisLib.BingMapsRestApi
{
    using System;

    public class BingMapsQueryResult
    {
        public BingMapsQueryResult(Response response)
            : this(response, null, null)
        {
        }

        public BingMapsQueryResult(Exception exception)
            : this(null, exception, null)
        {
        }

        public BingMapsQueryResult(Response response, object userState)
            : this(response, null, userState)
        {
        }

        public BingMapsQueryResult(Exception exception, object userState)
            : this(null, exception, userState)
        {
        }

        private BingMapsQueryResult(Response response, Exception exception, object userState)
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
        public Response Response { get; private set; }

        /// <summary>
        /// Gets the user state associated with this query.
        /// </summary>
        public object UserState { get; private set; }
    }
}
