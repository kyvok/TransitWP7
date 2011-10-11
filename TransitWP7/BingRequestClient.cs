using System;
using System.Net;

namespace TransitWP7
{
    public class BingRequestClient
    {
        HttpWebRequest request;

        public void Get()
        {
            //this.request = WebRequest.CreateHttp("jk");
            this.request = (HttpWebRequest)WebRequest.Create("jk");
            this.request.BeginGetResponse(new AsyncCallback(ResponseCallback), request);
        }

        private static void ResponseCallback(IAsyncResult asyncResult)
        {
            HttpWebRequest theR = (HttpWebRequest)asyncResult.AsyncState;
            HttpWebResponse response = (HttpWebResponse)theR.EndGetResponse(asyncResult);
            response.GetResponseStream();
        }
    }
}
