using System;
using System.Windows.Data;
using System.Windows.Media;

namespace DataGridControl
{
    public class MYColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            char text = value.ToString()[value.ToString().Length - 1];
            switch (text)
            {
                case (char)0x27C7:
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
