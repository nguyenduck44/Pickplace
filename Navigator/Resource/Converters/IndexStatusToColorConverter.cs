using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Data;

namespace NavigatorMachine.Resource.Converters
{
    public class IndexStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                switch (value)
                {
                    case "None":
                        return Brushes.Gray;
                    case "Processing":
                        return Brushes.Orange;
                    case "OK":
                        return Brushes.Green;
                    case "NG":
                        return Brushes.Red;
                }
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
