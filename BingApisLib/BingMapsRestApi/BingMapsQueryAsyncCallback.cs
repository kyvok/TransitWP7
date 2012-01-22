using System;
using System.Net;

namespace BingApisLib.BingMapsRestApi
{
    internal class BingMapsQueryAsyncCallback
    {
        private Action<BingMapsQueryResult> callback;
        private object userState;

        public BingMapsQueryAsyncCallback(Action<BingMapsQueryResult> callback, object userState)
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
            this.callback(new BingMapsQueryResult(exception, userState));
        }

        /// <summary>
        /// Invokes the callback with a result that contains the response.
        /// </summary>
        /// <param name="response">The Bing Maps response object.</param>
        public void Notify(Response response)
        {
            this.callback(new BingMapsQueryResult(response, userState));
        }
    }
}
