
using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bestiary.Model;

namespace Bestiary.ViewModel
{
    class BondLevelToIconConverter : IValueConverter
    {
        private BitmapImage m_Awakened;
        private BitmapImage m_Loyal;
        private BitmapImage m_Companion;
        private BitmapImage m_Inquisitive;
        private BitmapImage m_Relaxed;
        private BitmapImage m_Tolerant;
        private BitmapImage m_Wary;

        public BondLevelToIconConverter()
        {
            m_Awakened = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), "Awakened.png"));
            m_Loyal = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), "Loyal.png"));
            m_Companion = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), "Companion.png"));
            m_Inquisitive = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), "Inquisitive.png"));
            m_Relaxed = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), "Relaxed.png"));
            m_Tolerant = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), "Tolerant.png"));
            m_Wary = ImageLoader.LoadImage(Path.Combine(ApplicationPaths.GetDisplayIconDirectory(), "Wary.png"));
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BondingLevels? bondLevel = value as BondingLevels?;
            BitmapImage icon = new BitmapImage();
            switch(bondLevel)
            {
                case BondingLevels.Awakened:
                    icon = m_Awakened;
                    break;
                case BondingLevels.Loyal:
                    icon = m_Loyal;
                    break;
                case BondingLevels.Companion:
                    icon = m_Companion;
                    break;
                case BondingLevels.Inquisitive:
                    icon = m_Inquisitive;
                    break;
                case BondingLevels.Relaxed:
                    icon = m_Relaxed;
                    break;
                case BondingLevels.Tolerant:
                    icon = m_Tolerant;
                    break;
                case BondingLevels.Wary:
                    icon = m_Wary;
                    break;
            }
            return icon;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
