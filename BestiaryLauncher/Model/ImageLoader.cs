using System;
using System.Windows.Media.Imaging;

namespace BestiaryLauncher.Model
{
    static class ImageLoader
    {
        public static BitmapImage LoadImage(string path)
        {
            try
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                image.EndInit();
                return image;
            }
            catch
            {
                return null;
            }
        }
    }
}
