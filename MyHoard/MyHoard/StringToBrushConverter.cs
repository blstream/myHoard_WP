using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MyHoard
{
    public class StringToBrushConverter: IValueConverter
    {
       
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string pass = value as string;
            int i;
            Int32.TryParse((string)parameter,out i);
            SolidColorBrush brush = new SolidColorBrush();
            bool length = false;
            bool special = false;
            bool bothcases = false;
            bool numbers =false;
            if(!string.IsNullOrEmpty(pass))
            {
                length = pass.Length>=4;
                special = Regex.IsMatch(pass, "^(?=.*?[^A-Za-z0-9]).{4,}");
                bothcases = Regex.IsMatch(pass, "^(?=.*[a-z])(?=.*[A-Z]).{4,}");
                numbers = Regex.IsMatch(pass, "^(?=.*?[0-9]).{4,}");
            }
            switch(i)
            {
                case 0:
                    if (length)
                        brush = new SolidColorBrush(Color.FromArgb(255,255,192,2));
                    else
                        brush = new SolidColorBrush(Color.FromArgb(255, 70, 70, 70));
                    break;
                case 1:
                    if (special || bothcases || numbers)
                        brush = new SolidColorBrush(Color.FromArgb(255,255,132,1));
                    else
                        brush = new SolidColorBrush(Color.FromArgb(255, 54, 54, 54));
                    break;
                case 2:
                    if ((special && bothcases) || (special && numbers) || (bothcases && numbers))
                        brush = new SolidColorBrush(Color.FromArgb(255, 255, 109, 1));
                    else
                        brush = new SolidColorBrush(Color.FromArgb(255, 41, 41, 41));
                    break;
                case 3:
                    if (special && bothcases && numbers)
                        brush = new SolidColorBrush(Color.FromArgb(255, 255, 49, 1));
                    else
                        brush = new SolidColorBrush(Color.FromArgb(255, 32, 32, 32));
                    break;
            }
            return brush;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
