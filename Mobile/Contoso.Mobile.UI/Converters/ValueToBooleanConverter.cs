﻿using System;
using System.Collections;
using System.Globalization;
using Xamarin.Forms;

namespace Contoso.Mobile.UI.Converters
{
    public sealed class ValueToBooleanConverter : IValueConverter
    {
        public bool InvertValue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolean = true;

            if (value == null)
            {
                boolean = false;
            }
            else if (value is bool)
            {
                boolean = (bool)value;
            }
            else if (value is string)
            {
                boolean = !string.IsNullOrWhiteSpace(value.ToString());
            }
            else if (value is DateTime)
            {
                var val = (DateTime)value;
                boolean = !(val == DateTime.MinValue || val == DateTime.MaxValue);
            }
            else if (value is int)
            {
                var val = (int)value;
                boolean = !(val == int.MinValue || val == int.MaxValue);
            }
            else if (value is double)
            {
                var val = (double)value;
                boolean = !(val == double.MinValue || val == double.MaxValue || double.IsNaN(val));
            }
            else if (value is long)
            {
                var val = (long)value;
                boolean = !(val == long.MinValue || val == long.MaxValue);
            }
            else if (value is float)
            {
                var val = (float)value;
                boolean = !(val == float.MinValue || val == float.MaxValue);
            }
            else if (value is IEnumerable)
            {
                var val = value as IEnumerable;
                boolean = val.GetEnumerator().MoveNext();
            }

            if (this.InvertValue || parameter != null)
                boolean = !boolean;

            return boolean;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}