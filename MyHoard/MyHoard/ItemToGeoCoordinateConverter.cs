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
}
