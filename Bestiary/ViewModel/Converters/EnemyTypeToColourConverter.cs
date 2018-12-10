using Bestiary.Model;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Bestiary.ViewModel.Converters
{
    public class EnemyTypeToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if((EnemyTypes)value == EnemyTypes.Boss)
            {
                return Brushes.PeachPuff;
            }
            else
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
