﻿using Caliburn.Micro;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace MyHoard.Services
{
    public class RegistrationService
    {
        private MyHoardApi myHoardApi;
        private string backend;

        public RegistrationService(string backend)
        {
            this.backend = backend;
            this.myHoardApi = new MyHoardApi(ConfigurationService.Backends[backend]);
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
                    {
                        try
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.Created)
                            {
                                eventAggregator.Publish(new ServerMessage(true, Resources.AppResources.UserCreated));
                            }
                            else
                            {
                                JObject parsedResponse = JObject.Parse(response.Content);
                                string message = Resources.AppResources.ErrorOccurred + ": " + parsedResponse["error_message"];
                                eventAggregator.Publish(new ServerMessage(false, message));
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                            eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.ErrorOccurred));
                        }
                    }
                        
                });
            }
            else
            {
                eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.InternetConnectionError));
                return null;
            }
        }

        public RestRequestAsyncHandle Login(string userName, string password)
        {
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var request = new RestRequest("/oauth/token/", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Accept", "application/json");
                request.AddHeader("Content-type", "application/json");
                request.AddBody(new { username = userName, password = password, grant_type = "password" });
                return myHoardApi.ExecuteAsync(request, (response) =>
                {
                    if (response.ResponseStatus != ResponseStatus.Aborted)
                    {
                        ServerMessage serverMessage = new ServerMessage(false, Resources.AppResources.ErrorOccurred);
                        try
                        {
                            
                            if(response.StatusCode==System.Net.HttpStatusCode.OK)
                            {
                                JObject parsedResponse = JObject.Parse(response.Content);
                                if (String.IsNullOrWhiteSpace((string)parsedResponse["error_code"]))
                                {
                                    ConfigurationService configurationService = IoC.Get<ConfigurationService>();
                                    configurationService.Configuration.AccessToken = parsedResponse["access_token"].ToString();
                                    configurationService.Configuration.RefreshToken = parsedResponse["refresh_token"].ToString();
                                    configurationService.Configuration.UserName = userName;
                                    configurationService.Configuration.Password = password;
                                    configurationService.Configuration.Backend = backend;
                                    configurationService.Configuration.IsLoggedIn = true;
                                    configurationService.SaveConfig();
                                    serverMessage.IsSuccessfull = true;
                                    serverMessage.Message = Resources.AppResources.LoginSuccess;
                                }
                                else
                                {
                                    serverMessage.Message += ": " + parsedResponse["error_message"];
                                }
                            }

                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine(e.Message);
                        }
                        eventAggregator.Publish(serverMessage);
                    }
                });
            }
            else
            {
                eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.InternetConnectionError));
                return null;
            }
        }
    }
}
