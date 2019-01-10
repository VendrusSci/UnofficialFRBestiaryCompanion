using Bestiary.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bestiary.ViewModel.Converters
{
    class BondLevelToColourConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null)
            {
                BondingLevels level = (BondingLevels)value;
                switch(level)
                {
                    case BondingLevels.Awakened:
                        return Brushes.Gold;
                    case BondingLevels.Loyal:
                        return Brushes.Pink;
                    case BondingLevels.Companion:
                        return Brushes.MediumAquamarine;
                    case BondingLevels.Inquisitive:
                        return Brushes.SkyBlue;
                    case BondingLevels.Relaxed:
                        return Brushes.LightSalmon;
                    case BondingLevels.Tolerant:
                        return Brushes.PaleGreen;
                    case BondingLevels.Wary:
                        return Brushes.Tan;
                    default:
                        return Brushes.LightGray;
                }
            }
            
            return Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
