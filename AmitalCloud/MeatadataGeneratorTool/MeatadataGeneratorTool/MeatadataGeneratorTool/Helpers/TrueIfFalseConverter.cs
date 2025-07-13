using System;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using MeatadataGeneratorTool.CloseTablesData;
using System.Globalization;

namespace MeatadataGeneratorTool.Helpers
{
    public class TrueIfFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = (bool)value;
            if (result == true)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

       
    }
}
