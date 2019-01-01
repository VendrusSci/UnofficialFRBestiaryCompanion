using System.IO;
using System.Windows.Media.Imaging;

namespace Bestiary.ViewModel
{
    class UserGuideViewModel
    {
        public BitmapImage InterfaceImage { get; private set; }
        private string m_InterfaceImage = "GuideInterface.png";

        public UserGuideViewModel()
        {
            InterfaceImage = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), m_InterfaceImage));
        }
    }
}
