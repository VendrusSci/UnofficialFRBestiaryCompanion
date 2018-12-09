using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bestiary.ViewModel
{
    class LongestItemToWidthConverter : IValueConverter
    {
        public static readonly int DefaultWidth = 10;
        public static readonly int AveragePixelsPerChar = 8;
        public static readonly int ArrowWidth = 10;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Array)
            {
                var array = (Array) value;
                var longest = 0;
                foreach(var entry in array)
                {
                    var entryLength = entry.ToString().Length;
                    if(entryLength > longest)
                    {
                        longest = entryLength;
                    }
                }
                return longest * AveragePixelsPerChar + ArrowWidth;
            }
            else
            {
                return DefaultWidth * AveragePixelsPerChar + ArrowWidth;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
