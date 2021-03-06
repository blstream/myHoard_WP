﻿using Caliburn.Micro;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Phone.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MyHoard.Services
{
    public class RegistrationService
    {

        public RestRequestAsyncHandle Register(string email, string password, string backend)
        {
            MyHoardApi myHoardApi = new MyHoardApi(ConfigurationService.Backends[backend]);

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();

            var request = new RestRequest("/users/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddBody(new { email = email, password = password });
            return myHoardApi.ExecuteAsync(request, (response) =>
            {
                if (response.ResponseStatus != ResponseStatus.Aborted)
                {
                    try
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            Login(email, password, true, backend);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            string message = Resources.AppResources.GeneralError + ": " + Resources.AppResources.ServerNotFound + "\n" + Resources.AppResources.CheckConnection;
                            eventAggregator.Publish(new ServerMessage(false, message));
                        }
                        else if (!String.IsNullOrWhiteSpace(response.Content))
                        {
                            JObject parsedResponse = JObject.Parse(response.Content);
                            string message = Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"] + "\n";
                            foreach (var jObject in parsedResponse["errors"])
                            {
                                var jtoken = jObject.First;

                                while (jtoken != null)//loop through columns
                                {
                                    message += jtoken.ToString() + "\n";

                                    jtoken = jtoken.Next;
                                }
                            }
                            eventAggregator.Publish(new ServerMessage(false, message));

                        }
                        else
                        {
                            eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                        eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError));
                    }
                }

            });
        }



        public RestRequestAsyncHandle Login(string email, string password, bool keepLogged, string backend)
        {

            MyHoardApi myHoardApi = new MyHoardApi(ConfigurationService.Backends[backend]);
            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            ConfigurationService configurationService = IoC.Get<ConfigurationService>();

            var request = new RestRequest("/oauth/token/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddBody(new { email = email, password = password, grant_type = "password" });
            return myHoardApi.ExecuteAsync(request, (response) =>
            {
                if (response.ResponseStatus != ResponseStatus.Aborted)
                {
                    ServerMessage serverMessage = new ServerMessage(false, Resources.AppResources.AuthenticationError);

                    JObject parsedResponse = new JObject();
                    try
                    {
                        parsedResponse = JObject.Parse(response.Content);
                    }
                    catch (Newtonsoft.Json.JsonException)
                    {
                        serverMessage.Message = Resources.AppResources.GeneralError;
                    }
                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.OK:
                        case System.Net.HttpStatusCode.Created:
                            configurationService.Configuration.AccessToken = parsedResponse["access_token"].ToString();
                            configurationService.Configuration.RefreshToken = parsedResponse["refresh_token"].ToString();
                            configurationService.Configuration.ServerId = parsedResponse["user_id"].ToString();
                            configurationService.Configuration.Password = password;
                            configurationService.Configuration.Email = email;
                            configurationService.Configuration.KeepLogged = keepLogged;
                            configurationService.Configuration.Backend = backend;
                            configurationService.Configuration.IsLoggedIn = true;
                            configurationService.SaveConfig();
                            serverMessage.IsSuccessfull = true;
                            serverMessage.Message = Resources.AppResources.LoginSuccess;
                            break;
                        case System.Net.HttpStatusCode.Unauthorized:
                        case System.Net.HttpStatusCode.Forbidden:
                            configurationService.Logout();
                            serverMessage.Message += ": " + parsedResponse["error_message"] + "\n" + parsedResponse["errors"];
                            break;
                        default:
                            configurationService.Logout();
                            serverMessage.Message += ": " + parsedResponse["error_message"] + "\n" + parsedResponse["errors"];
                            break;
                    }

                    eventAggregator.Publish(serverMessage);
                }
            });


        }

        public async Task<bool> RefreshToken()
        {
            bool success = false;

            IEventAggregator eventAggregator = IoC.Get<IEventAggregator>();
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                var request = new RestRequest("/oauth/token/", Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("Content-type", "application/json");
                ConfigurationService configurationService = IoC.Get<ConfigurationService>();
                request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
                request.AddBody(new
                {
                    email = configurationService.Configuration.Email,
                    password = configurationService.Configuration.Password,
                    grant_type = "refresh_token",
                    refresh_token = configurationService.Configuration.RefreshToken
                });

                MyHoardApi myHoardApi = new MyHoardApi(ConfigurationService.Backends[configurationService.Configuration.Backend]);

                IRestResponse response = await myHoardApi.Execute(request);
                if (response.ResponseStatus != ResponseStatus.Aborted)
                {
                    try
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Created)
                        {
                            JObject parsedResponse = JObject.Parse(response.Content);
                            if (String.IsNullOrWhiteSpace((string)parsedResponse["error_code"]))
                            {
                                configurationService.Configuration.AccessToken = parsedResponse["access_token"].ToString();
                                configurationService.Configuration.RefreshToken = parsedResponse["refresh_token"].ToString();
                                configurationService.Configuration.ServerId = parsedResponse["user_id"].ToString();
                                configurationService.SaveConfig();
                                success = true;
                            }
                            else
                            {
                                configurationService.Logout();
                                ServerMessage serverMessage = new ServerMessage(false, Resources.AppResources.AuthenticationError + ": " + parsedResponse["error_message"]);
                                eventAggregator.Publish(serverMessage);
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.Message);
                    }

                }
            }

            else
            {
                ServerMessage serverMessage = new ServerMessage(false, Resources.AppResources.InternetConnectionError);
                eventAggregator.Publish(serverMessage);
            }

            return success;
        }
    }
}
