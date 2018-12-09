using Bestiary.Model;
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Bestiary.ViewModel
{
    public class FlightToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var element = (Flights)value;
            return ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory() , element.ToString() + ".png"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
