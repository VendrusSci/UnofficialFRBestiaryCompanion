using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Bestiary.ViewModel.Converters
{
    class BookmarkStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BookmarkState state = (BookmarkState)value;
            string png = state == BookmarkState.Bookmarked ? "Bookmark.png" : "NoBookmark.png";

            return ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), png));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
