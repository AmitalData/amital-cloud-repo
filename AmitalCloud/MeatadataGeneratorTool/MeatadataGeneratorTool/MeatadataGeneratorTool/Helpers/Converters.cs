using System;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;

namespace MeatadataGeneratorTool.Helpers
{
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;

            var border = new Border();// { Background = Brushes.Red, Width=100, Height=50 };
            border.Child = value as DataGrid;
            return border;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
