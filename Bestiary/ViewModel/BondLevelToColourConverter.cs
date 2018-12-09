
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Bestiary.Model;

namespace Bestiary.ViewModel
{
    class BondLevelToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            BondingLevels? bondLevel = value as BondingLevels?;
            SolidColorBrush brush = new SolidColorBrush(bondLevel == BondingLevels.Awakened ? Colors.PeachPuff : Colors.Transparent);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
