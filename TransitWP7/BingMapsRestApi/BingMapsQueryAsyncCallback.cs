//TODO: copyright info

namespace TransitWP7.BingMapsRestApi
{
    using System;

    internal class BingMapsQueryAsyncCallback
    {
        private Action<BingMapsQueryResult> callback;

        public BingMapsQueryAsyncCallback(Action<BingMapsQueryResult> callback)
        {
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            this.callback = callback;
        }

        public void Notify(Exception exception)
        {
            this.callback(new BingMapsQueryResult(exception));
        }

        public void Notify(Response result)
        {
            this.callback(new BingMapsQueryResult(result));
        }
    }
}
