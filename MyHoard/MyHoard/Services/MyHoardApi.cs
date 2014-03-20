using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class MyHoardApi
    {
        private RestClient client;

        public MyHoardApi(string baseUrl)
        {

            client = new RestClient();
            client.BaseUrl = baseUrl;
        }

        public RestRequestAsyncHandle ExecuteAsync(RestRequest request, Action<IRestResponse> callback)
        {
            return client.ExecuteAsync(request, callback);
        }

        public Task<IRestResponse> Execute(RestRequest restRequest)
        {
            var tcs = new TaskCompletionSource<IRestResponse>();
            client.ExecuteAsync(restRequest, (restResponse, asyncHandle) =>
            {
                tcs.SetResult(restResponse);
            });
            return tcs.Task;
        }

    }
}
