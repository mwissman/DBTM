using System;
using System.Globalization;
using System.Windows.Data;

namespace DBTM.UI.Converters
{
    public class TitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length==2 && values[0] is bool && values[1] is string)
            {
                return string.Format("Database Transition Manager - {0}{1}", values[1], (bool)values[0] ? "" : "*");
            }

            return "Database Transition Manager";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}