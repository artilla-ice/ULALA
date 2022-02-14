using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Windows.UI.Xaml.Data;

namespace ULALA.UI.Core.Converters
{
    public class StringCurrencyFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                double number = 0;      
                if (value.GetType() == typeof(string))
                {
                    var stringValue = (string)value;
                    if (!string.IsNullOrEmpty(stringValue) && char.IsDigit(stringValue[0]))
                        number = double.Parse(stringValue, CultureInfo.InvariantCulture);
                }
                else
                    number = (double)value;

                var formattedString = String.Format("${0}", number.ToString("N"));

                return formattedString;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                var stringValue = value.ToString();
                stringValue = stringValue.Replace("$", string.Empty);

                return stringValue;
            }

            return null;
        }
    }
}
