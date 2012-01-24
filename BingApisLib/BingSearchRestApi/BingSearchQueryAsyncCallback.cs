using System;
using System.Net;

namespace BingApisLib.BingSearchRestApi
{
    internal class BingSearchQueryAsyncCallback
    {
        private Action<BingSearchQueryResult> callback;
        private object userState;

        public BingSearchQueryAsyncCallback(Action<BingSearchQueryResult> callback, object userState)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            this.callback = callback;
            this.userState = userState;
        }

        public HttpWebRequest HttpWebRequest { get; private set; }

        /// <summary>
        /// Invokes the callback with a result that contains an exception.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        public void Notify(Exception exception)
        {
            this.callback(new BingSearchQueryResult(exception, this.userState));
        }

        /// <summary>
        /// Invokes the callback with a result that contains the response.
        /// </summary>
        /// <param name="response">The Bing Maps response object.</param>
        public void Notify(SearchResponse response)
        {
            this.callback(new BingSearchQueryResult(response, this.userState));
        }
    }
}
