using Caliburn.Micro;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class RegistrationService
    {
        private MyHoardApi myHoardApi;

        public RegistrationService(string baseUrl)
        {
            this.myHoardApi = new MyHoardApi(baseUrl);
        }

        public RestRequestAsyncHandle Register(string userName, string email, string password)
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var request = new RestRequest("/users/", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-type", "application/json");
                request.AddBody(new { username = userName, email = email, password = password });
                return myHoardApi.ExecuteAsync(request, (response) =>
                {
                    if (response.ResponseStatus != ResponseStatus.Aborted)
                        eventAggregator.Publish(response);
                });
            }
            else
            {
                eventAggregator.Publish(new ServiceErrorMessage(Resources.AppResources.InternetConnectionError));
                return null;
            }
        }
    }
}
