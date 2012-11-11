namespace GoogleApisLib.GoogleMapsApi
{
    using System;

    public class GoogleMapsQueryResult
    {
        public GoogleMapsQueryResult(DirectionsResult response)
            : this(response, null)
        {
        }

        public GoogleMapsQueryResult(Exception exception)
            : this(null, exception)
        {
        }

        private GoogleMapsQueryResult(DirectionsResult response, Exception exception)
        {
            this.Response = response;
            this.Error = exception;
        }

        /// <summary>
        /// Gets a value indicating which exception occured if any.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Gets the Google Maps Response object returned by this query.
        /// </summary>
        public DirectionsResult Response { get; private set; }
    }
}
