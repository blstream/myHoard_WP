using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MyHoard
{
    public class LowerUpperCaseConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            string s = value as string;
            if (!string.IsNullOrEmpty(s))
            {
                int i;
                Int32.TryParse((string)parameter, out i);
                if(i==1)
                {
                    s = s.ToUpper();
                }
                else
                {
                    s = s.ToLower();
                }
            }
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    
    }
}
