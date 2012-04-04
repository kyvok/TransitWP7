namespace BingApisLib.BingSearchRestApi
{
    using System;
    
    internal class BingSearchQueryAsyncCallback
    {
        private readonly Action<BingSearchQueryResult> _callback;
        private readonly object _userState;

        public BingSearchQueryAsyncCallback(Action<BingSearchQueryResult> callback, object userState)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            this._callback = callback;
            this._userState = userState;
        }

        /// <summary>
        /// Invokes the _callback with a result that contains an exception.
        /// </summary>
        /// <param name="exception">The exception that occured.</param>
        public void Notify(Exception exception)
        {
            this._callback(new BingSearchQueryResult(exception, this._userState));
        }

        /// <summary>
        /// Invokes the _callback with a result that contains the response.
        /// </summary>
        /// <param name="response">The Bing Maps response object.</param>
        public void Notify(SearchResponse response)
        {
            this._callback(new BingSearchQueryResult(response, this._userState));
        }
    }
}
