namespace GoogleApisLib.GooglePlacesApi
{
    using System;

    internal class GooglePlacesQueryAsyncCallback
    {
        private readonly Action<GooglePlacesQueryResult> _callback;
        private readonly object _userState;

        public GooglePlacesQueryAsyncCallback(Action<GooglePlacesQueryResult> callback, object userState)
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
            this._callback(new GooglePlacesQueryResult(exception, this._userState));
        }

        /// <summary>
        /// Invokes the _callback with a result that contains the response.
        /// </summary>
        /// <param name="response">The Google Places response object.</param>
        public void Notify(SearchResponse response)
        {
            this._callback(new GooglePlacesQueryResult(response, this._userState));
        }
    }
}
