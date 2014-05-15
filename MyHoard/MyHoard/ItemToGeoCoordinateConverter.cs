using Microsoft.Phone.Maps.Controls;
using MyHoard.Models;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MyHoard
{
    /// <summary>
    /// Returns coordinates from Item.
    /// </summary>
    public class ItemToGeoCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Item valueAsItem = value as Item;
            if (valueAsItem  == null)
                return new GeoCoordinate();

            var retVal = new GeoCoordinate((double)valueAsItem.LocationLat, (double)valueAsItem.LocationLat);// This accepts only range -90 to 90, I don't get why
            retVal.Latitude = (double)valueAsItem.LocationLat;
            retVal.Longitude = (double)valueAsItem.LocationLng;

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Returns maplayers with point in the midde from Item.
    /// Should be used to display a circle on a map from Item's location
    /// </summary>
    public class ItemToMapLayersListWithPointConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Item valueAsItem = value as Item;
            if (valueAsItem == null)
                return null;


            var retVal = new List<MapLayer>();
            MapLayer mapLayer = new MapLayer();
            retVal.Add(mapLayer);

            MapOverlay overlay = new MapOverlay();
            overlay.PositionOrigin = new Point(1,1);
            overlay.GeoCoordinate = new GeoCoordinate(){
                Latitude = valueAsItem.LocationLat,
                Longitude = valueAsItem.LocationLng
            };
            mapLayer.Add(overlay);

            Ellipse circle = new Ellipse()
            {
                Fill = new SolidColorBrush(new Color() {B = 255}),
                Height = 20,
                Width = 20,
                Opacity = 50
            };
            overlay.Content = circle;

            return retVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



}
