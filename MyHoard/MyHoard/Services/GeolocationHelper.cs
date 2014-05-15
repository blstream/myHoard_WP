using Microsoft.Phone.Maps.Services;
using MyHoard.Models;
using MyHoard.Resources;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.Devices.Geolocation;

namespace MyHoard.Services
{
    public static class GeolocationHelper
    {
        public static async Task GetCurrentLocation(Item item)
        {
            Geolocator geolocator = new Geolocator();
            geolocator.DesiredAccuracyInMeters = 50;

            try
            {
                Geoposition geoposition = await geolocator.GetGeopositionAsync(
                    maximumAge: TimeSpan.FromMinutes(5),
                    timeout: TimeSpan.FromSeconds(10)
                    );

                item.LocationLat = (float)geoposition.Coordinate.Latitude;
;
                item.LocationLng = (float) geoposition.Coordinate.Longitude;
                item.LocationSet = true;
            }
            catch (Exception ex)
            {
                if ((uint)ex.HResult == 0x80004004)
                {
                    // the application does not have the right capability or the location master switch is off
                    MessageBox.Show(AppResources.CannotObtainLocation);
                }
                //else
                {
                    // something else happened acquring the location
                    MessageBox.Show(AppResources.CannotObtainLocation);
                }
            }
        }

        public static void ClearLocation(Item item)
        {
            item.LocationLat = 0;
            item.LocationLng = 0;
            item.LocationSet = false;
        }

        /// <summary>
        /// TODO - doesn't really work properly.
        /// Fills Location name using reverse geodata
        /// </summary>
        /// <param name="item"></param>
        public static void GetLocationName(Item item)
        {
            ReverseGeocodeQuery query = new ReverseGeocodeQuery(){
            GeoCoordinate = new GeoCoordinate(item.LocationLat,item.LocationLng)};
            query.QueryCompleted += (s, e) =>
            {
                if (e.Error != null ||
                    e.Result.Count < 1)
                    return;
                item.LocationName = e.Result[0].Information.Address.ToString();
            };

            query.QueryAsync();
        }
    }
}
