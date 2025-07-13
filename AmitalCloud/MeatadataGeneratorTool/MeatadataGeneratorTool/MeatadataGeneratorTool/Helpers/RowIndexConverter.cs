using System;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using MeatadataGeneratorTool.CloseTablesData;
using System.Globalization;

namespace MeatadataGeneratorTool.Helpers
{
    public class RowIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Row row = value as Row;
            string index = parameter as string;
            return row[index];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
