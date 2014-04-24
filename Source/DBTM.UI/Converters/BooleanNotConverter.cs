﻿using System;
using System.Globalization;
using System.Windows.Data;

namespace DBTM.UI.Converters
{
    public class BooleanNotConverter : IValueConverter
    {

        public object Convert(object value, Type targetType,
                              object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            return !((bool)value);
        }
    }
}