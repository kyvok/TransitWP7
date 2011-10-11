//TODO: copyright info

namespace TransitWP7.BingMapsRestApi
{
    using System;

    public class BingMapsQueryResult
    {
        public Exception Error { get; private set; }
        public Response Result { get; private set; }

        public BingMapsQueryResult(Response result)
        {
            this.Result = result;
        }

        public BingMapsQueryResult(Exception exception)
        {
            this.Error = exception;
        }
    }
}
