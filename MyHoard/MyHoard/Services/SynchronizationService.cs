using Caliburn.Micro;
using MyHoard.Models;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class SynchronizationService
    {
        private async Task pushCollections(CancellationToken cancellationToken)
        {
            CollectionService collectionService = IoC.Get<CollectionService>();
            ConfigurationService configurationService = IoC.Get<ConfigurationService>();
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();

            MyHoardApi myHoardApi = new MyHoardApi(ConfigurationService.Backends[configurationService.Configuration.Backend]);

            RegistrationService r = new RegistrationService();
            bool refresh = await r.RefreshToken();
            if (!refresh)
                return;


            foreach (Collection c in collectionService.CollectionList())
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (!c.IsPrivate)
                {
                    string id = "";
                    bool synced = false;
                    switch (configurationService.Configuration.Backend)
                    {
                        case "Python":
                            id = c.PythonId;
                            synced = c.PythonIsSynced;
                            break;
                        case "Java1":
                            id = c.Java1Id;
                            synced = c.Java1IsSynced;
                            break;
                        case "Java2":
                            id = c.Java2Id;
                            synced = c.Java2IsSynced;
                            break;
                    }

                    if (String.IsNullOrWhiteSpace(id))
                    {
                        var request = new RestRequest("/collections/", Method.POST);
                        request.RequestFormat = DataFormat.Json;
                        request.AddHeader("Accept", "application/json");
                        request.AddHeader("Content-type", "application/json");
                        request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
                        request.AddBody(new { name = c.Name, description = c.Description, tags = c.TagList });

                        var response = await myHoardApi.Execute(request);
                        JObject parsedResponse = JObject.Parse(response.Content);
                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.Created:
                                switch (configurationService.Configuration.Backend)
                                {
                                    case "Python":
                                        c.PythonId = (string)parsedResponse["id"];
                                        c.PythonIsSynced = true;
                                        break;
                                    case "Java1":
                                        c.Java1Id = (string)parsedResponse["id"];
                                        c.Java1IsSynced = true;
                                        break;
                                    case "Java2":
                                        c.Java2Id = (string)parsedResponse["id"];
                                        c.Java2IsSynced = true;
                                        break;
                                }

                                collectionService.ModifyCollection(c);
                                break;
                            case System.Net.HttpStatusCode.Forbidden:
                                configurationService.Logout();
                                eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                                return;
                            default:
                                eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                                    + "\n" + parsedResponse["errors"]));
                                return;
                        }
                    }
                    else if (!synced)
                    {
                        var request = new RestRequest("/collections/" + id, Method.PUT);
                        request.RequestFormat = DataFormat.Json;
                        request.AddHeader("Accept", "application/json");
                        request.AddHeader("Content-type", "application/json");
                        request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
                        request.AddBody(new { name = c.Name, description = c.Description, tags = c.TagList });

                        var response = await myHoardApi.Execute(request);
                        JObject parsedResponse = JObject.Parse(response.Content);
                        switch (response.StatusCode)
                        {
                            case System.Net.HttpStatusCode.OK:
                                c.PythonIsSynced = true;
                                collectionService.ModifyCollection(c);
                                break;
                            case System.Net.HttpStatusCode.Forbidden:
                                configurationService.Logout();
                                eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                                return;
                            default:
                                eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                                    + "\n" + parsedResponse["errors"]));
                                return;
                        }
                    }


                }
                
            }

            eventAggregator.Publish(new ServerMessage(true, Resources.AppResources.Synchronized));
        }

        public async Task PushCollections(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() => pushCollections(cancellationToken));
        }
    }
}
