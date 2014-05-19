using Caliburn.Micro;
using Microsoft.Phone;
using MyHoard.Models;
using MyHoard.Models.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MyHoard.Services
{
    public class SynchronizationService
    {

        private readonly IEventAggregator eventAggregator;
        private readonly CollectionService collectionService;
        private readonly ConfigurationService configurationService;
        private readonly ItemService itemService;
        private readonly MediaService mediaService;
        private readonly MyHoardApi myHoardApi;
        private string backend;

        public SynchronizationService()
        {
            eventAggregator = IoC.Get<IEventAggregator>();
            collectionService = IoC.Get<CollectionService>();
            configurationService = IoC.Get<ConfigurationService>();
            backend = configurationService.Configuration.Backend;
            myHoardApi = new MyHoardApi(ConfigurationService.Backends[backend]);
            itemService = IoC.Get<ItemService>();
            mediaService = IoC.Get<MediaService>();
        }

        private async Task syncDatabase(CancellationToken cancellationToken)
        {
            // Temporary commented out due to problem with backend

            //RegistrationService r = new RegistrationService();
            //bool refresh = await r.RefreshToken();
            //if (!refresh)
            //{
            //    configurationService.Logout();
            //    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
            //    return;
            //}

            List<Collection> collections = collectionService.CollectionList(true);

            foreach (Collection c in collections)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }
                
                bool success;
                if (!c.IsPrivate)
                {
                    if (String.IsNullOrWhiteSpace(c.ServerId) && !c.ToDelete)
                    {
                        success = await AddCollection(c);
                        if (!success)
                            return;
                    }
                    else if (!c.IsSynced && !c.ToDelete)
                    {
                        success = await modifyCollection(c);
                        if (!success)
                            return;
                    }
                    else if (c.ToDelete && !String.IsNullOrEmpty(c.ServerId))
                    {
                        success = await DeleteCollection(c);
                        if (!success)
                            return;
                    }

                    if (!c.IsPrivate && !c.ToDelete)
                    {
                        foreach (Item i in itemService.ItemList(c.Id, true))
                        {

                            foreach (Media m in mediaService.MediaList(i.Id, false, false))
                            {
                                
                                if (String.IsNullOrWhiteSpace(m.ServerId) && !m.ToDelete)
                                {
                                    success = await addMedia(m);
                                    if (!success)
                                        return;
                                }
                            }
                           
                            if (String.IsNullOrWhiteSpace(i.ServerId) && !i.ToDelete)
                            {
                                success = await addItem(i, c.ServerId);
                                if (!success)
                                    return;
                            }
                            else if (!i.IsSynced && !i.ToDelete)
                            {
                                success = await modifyItem(i, c.ServerId);
                                if (!success)
                                    return;
                            }
                            else if (i.ToDelete && !string.IsNullOrEmpty(i.ServerId))
                            {
                                success = await deleteItem(i);
                                if (!success)
                                    return;
                            }
                        }
                    }
                }
                else
                {
                    if(!string.IsNullOrWhiteSpace(c.ServerId))
                    {
                        success = await DeleteCollection(c, true);
                        if (!success)
                            return;
                    }
                     
                }
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            List<ServerCollection> serverColections = await getCollections();
            if (serverColections != null)
            {
                foreach (ServerCollection serverCollection in serverColections)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    Collection localCollection = collections.FirstOrDefault(c => c.ServerId == serverCollection.id);
                    localCollection = syncCollectionFromServer(localCollection, serverCollection);

                    List<Item> items = itemService.ItemList(localCollection.Id);

                    List<ServerItem> serverItems = await getItems(serverCollection);
                    if (serverItems != null)
                        foreach (ServerItem serverItem in serverItems)
                        {
                            Item localItem = items.FirstOrDefault(i => i.ServerId == serverItem.id);
                            localItem = syncItemFromServer(localItem, serverItem, localCollection.Id);
                            await syncMedia(localItem, serverItem);
                        }
                    else
                        return;
                    foreach (Item localItem in items)
                    {
                        ServerItem serverItem = serverItems.FirstOrDefault(i => i.id == localItem.ServerId);
                        if (serverItem == null)
                        {
                            localItem.ServerId=null;
                            itemService.ModifyItem(localItem);
                            itemService.DeleteItem(localItem);
                        }
                    }
                }
            }
            else 
                return;
            eventAggregator.Publish(new ServerMessage(true, Resources.AppResources.Synchronized));
        }

        private void savePicture(byte[] image, int localItemId, string serverMediaId)
        {
            Media m = mediaService.SavePictureToIsolatedStorage(
                            new Media()
                            {
                                Image = mediaService.ByteArrayToWriteableBitmap(image),
                                ItemId = localItemId
                            });
            m.ServerId = serverMediaId;
            m.IsSynced = true;
            mediaService.AddMedia(m);
        }

        private async Task syncMedia(Item localItem, ServerItem serverItem)
        {
            List<Media> media = mediaService.MediaList(localItem.Id, false);

            foreach (ServerMedia serverMedia in serverItem.media)
            {
                Media localMedia = media.FirstOrDefault(m => m.ServerId == serverMedia.id);
                if (localMedia == null)
                {
                    byte[] image = await GetImage(serverMedia);
                    if (image != null)
                    {
                        
                        Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            savePicture(image, localItem.Id, serverMedia.id);
                        });
                    }
                    else
                        return;
                }
            }
            foreach (Media localMedia in media)
            {
                ServerMedia serverMedia = serverItem.media.FirstOrDefault(m => m.id == localMedia.ServerId);
                if (serverMedia == null)
                {
                    localMedia.ServerId = null;
                    localMedia.ToDelete = true;
                    mediaService.ModifyMedia(localMedia);
                }
            }
        }

        private async Task<byte[]> GetImage(ServerMedia serverMedia)
        {
            var request = new RestRequest("/media/" + serverMedia.id + "/", Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    return response.RawBytes;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return null;
                default:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                        parsedResponse = JObject.Parse(response.Content);
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return null;
            }
        }

        private Item syncItemFromServer(Item localItem, ServerItem serverItem, int collectionId)
        {
            if (localItem == null)
            {
                localItem = new Item()
                {
                    Name = serverItem.name,
                    Description = serverItem.description,
                    CollectionId = collectionId,
                    ModifiedDate = serverItem.ModifiedDate(),
                };
                localItem.LocationSet = serverItem.location != null;
                if(localItem.LocationSet)
                {
                    localItem.LocationLat = serverItem.location.lat;
                    localItem.LocationLng = serverItem.location.lng;
                }

                localItem.ServerId = serverItem.id;
                localItem.IsSynced = true;
                return itemService.AddItem(localItem);
            }
            else if (DateTime.Compare(serverItem.ModifiedDate(), localItem.ModifiedDate) > 0)
            {
                localItem.Name = serverItem.name;
                localItem.Description = serverItem.description;

                localItem.LocationSet = serverItem.location != null;
                if (localItem.LocationSet)
                {
                    localItem.LocationLat = serverItem.location.lat;
                    localItem.LocationLng = serverItem.location.lng;
                }

                localItem.ModifiedDate = serverItem.ModifiedDate();
                localItem.IsSynced = true;
                return itemService.ModifyItem(localItem);
            }
            return localItem;
        }

        private Collection syncCollectionFromServer(Collection localCollection, ServerCollection serverCollection)
        {
            if (localCollection == null)
            {
                localCollection = new Collection()
                {
                    Name = serverCollection.name,
                    Description = serverCollection.description,
                    TagList = serverCollection.tags,
                    ModifiedDate = serverCollection.ModifiedDate()
                };
                localCollection.IsSynced = true;
                localCollection.ServerId = serverCollection.id;
                localCollection = collectionService.AddCollection(localCollection);
            }
            else if (DateTime.Compare(serverCollection.ModifiedDate(), localCollection.ModifiedDate) > 0)
            {
                localCollection.Description = serverCollection.description;
                localCollection.ModifiedDate = serverCollection.ModifiedDate();
                localCollection.Name = serverCollection.name;
                localCollection.TagList = serverCollection.tags;
                localCollection.IsSynced = true;
                collectionService.ModifyCollection(localCollection);
            }
            return localCollection;
        }

        private async Task<bool> addMedia(Media m)
        {
            var request = new RestRequest("/media/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
            request.AddFile("image", mediaService.GetPictureAsByteArray(m), m.FileName, "image");

            var response = await myHoardApi.Execute(request);

            JObject parsedResponse = new JObject();
            if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                parsedResponse = JObject.Parse(response.Content);
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                    m.ServerId = (string)parsedResponse["id"];
                    mediaService.ModifyMedia(m);
                    return true;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return false;
                default:
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return false;
            }
        }

        public async Task<bool> DeleteCollection(Collection c, bool isPrivate=false)
        {
            var request = new RestRequest("/collections/" + c.ServerId + "/", Method.DELETE);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);

            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();
            if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                parsedResponse = JObject.Parse(response.Content);
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NoContent:
                case System.Net.HttpStatusCode.NotFound:
                    c.ServerId = null;
                    collectionService.ModifyCollection(c);
                    if(isPrivate)
                    {
                        foreach(Item i in itemService.ItemList(c.Id))
                        {
                            i.ServerId = null;
                            itemService.ModifyItem(i);
                            foreach(Media m in mediaService.MediaList(i.Id,false))
                            {
                                m.ServerId = null;
                                mediaService.ModifyMedia(m);
                            }
                        }
                    }
                    else
                        collectionService.DeleteCollection(c);
                    return true;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return false;
                default:
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return false;
            }
        }


        private async Task<bool> modifyCollection(Collection c)
        {
            var request = new RestRequest("/collections/" + c.ServerId + "/", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
            request.AddBody(new { name = c.Name, description = c.Description, tags = c.TagList });

            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    ServerCollection sc = JsonConvert.DeserializeObject<ServerCollection>(response.Content);
                    c.ModifiedDate = sc.ModifiedDate();
                    c.IsSynced = true;
                    collectionService.ModifyCollection(c);
                    return true;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return false;
                default:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                        parsedResponse = JObject.Parse(response.Content);
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return false;
            }
        }

        public async Task<bool> AddCollection(Collection c)
        {
            var request = new RestRequest("/collections/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
            request.AddBody(new { name = c.Name, description = c.Description, tags = c.TagList });

            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                case System.Net.HttpStatusCode.OK:
                    ServerCollection sc = JsonConvert.DeserializeObject<ServerCollection>(response.Content);
                    c.ServerId = sc.id;
                    c.IsSynced = true;
                    c.ModifiedDate = sc.ModifiedDate();
                    collectionService.ModifyCollection(c);
                    return true;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return false;
                default:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                        parsedResponse = JObject.Parse(response.Content);
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return false;
            }
        }

        private async Task<bool> addItem(Item i, string parentServerId)
        {
            var request = new RestRequest("/items/", Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
            request.AddBody(new
            {
                name = i.Name,
                description = i.Description,
                location = i.LocationSet ? new { lat = i.LocationLat, lng = i.LocationLng } : null,
                media = mediaService.MediaStringList(i.Id),
                collection = parentServerId
            });

            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                    ServerItem si = JsonConvert.DeserializeObject<ServerItem>(response.Content);
                    i.ServerId = si.id;
                    i.IsSynced = true;
                    i.ModifiedDate = si.ModifiedDate();
                    itemService.ModifyItem(i);
                    return true;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return false;
                default:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                        parsedResponse = JObject.Parse(response.Content);
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return false;
            }
        }

        private async Task<bool> modifyItem(Item i, string parentServerId)
        {
            var request = new RestRequest("/items/" + i.ServerId + "/", Method.PUT);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
            // Temporary changed due to problem with backend
            request.AddBody(new
            {
                name = i.Name,
                description = i.Description,
                location = i.LocationSet ? new { lat = i.LocationLat, lng = i.LocationLng } : null,
                media = mediaService.MediaStringList(i.Id),
                collection = parentServerId
            });

            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    ServerItem si = JsonConvert.DeserializeObject<ServerItem>(response.Content);
                    i.IsSynced=true;
                    i.ModifiedDate = si.ModifiedDate();
                    itemService.ModifyItem(i);
                    return true;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return false;
                default:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                        parsedResponse = JObject.Parse(response.Content);
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return false;
            }
        }

        private async Task<bool> deleteItem(Item i)
        {
            var request = new RestRequest("/items/" + i.ServerId + "/", Method.DELETE);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-type", "application/json");
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);

            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();
            if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                parsedResponse = JObject.Parse(response.Content);
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.NoContent:
                case System.Net.HttpStatusCode.NotFound:
                    i.ServerId = null;
                    itemService.ModifyItem(i);
                    itemService.DeleteItem(i);
                    return true;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return false;
                default:
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return false;
            }
        }

        private async Task<List<ServerItem>> getItems(ServerCollection serverCollection)
        {
            var request = new RestRequest("/collections/" + serverCollection.id + "/items/"
                + "?timestamp" + DateTime.Now.ToString(), Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);
            //request.AddHeader("Content-type", "application/json");
            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();

            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                    {
                        return JsonConvert.DeserializeObject<List<ServerItem>>(response.Content);
                    }
                    else
                        return null;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return null;
                default:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                        parsedResponse = JObject.Parse(response.Content);
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return null;
            }
        }

        private async Task<List<ServerCollection>> getCollections()
        {
            var request = new RestRequest("/collections/" + "?timestamp" + DateTime.Now.ToString(), Method.GET);
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Authorization", configurationService.Configuration.AccessToken);

            var response = await myHoardApi.Execute(request);
            JObject parsedResponse = new JObject();
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.OK:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                    {
                        var collections = JsonConvert.DeserializeObject<List<ServerCollection>>(response.Content);
                        return collections.Where(x => x.owner == configurationService.Configuration.ServerId).ToList();
                    }
                    else
                        return null;
                case System.Net.HttpStatusCode.Unauthorized:
                case System.Net.HttpStatusCode.Forbidden:
                    configurationService.Logout();
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.AuthenticationError));
                    return null;
                default:
                    if (response.Content.StartsWith("{") || response.Content.StartsWith("["))
                        parsedResponse = JObject.Parse(response.Content);
                    eventAggregator.Publish(new ServerMessage(false, Resources.AppResources.GeneralError + ": " + parsedResponse["error_message"]
                        + "\n" + parsedResponse["errors"]));
                    return null;
            }
        }

        public async Task SyncDatabase(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(() => syncDatabase(cancellationToken));
        }
    }
}
