using Bestiary.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Bestiary.ViewModel.Converters
{
    public class OwnedToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            OwnershipStatus status = (OwnershipStatus)value;
            if (status == OwnershipStatus.Owned)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
