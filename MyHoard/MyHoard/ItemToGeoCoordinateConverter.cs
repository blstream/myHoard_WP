using MyHoard.Models;
using System;
using System.Device.Location;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

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
            return new GeoCoordinate(// This accepts only range -90 to 90, I don't get why
                valueAsItem.LocationLat / 2, valueAsItem.LocationLng / 2);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
