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
            BondingLevels? level = value as BondingLevels?;
            switch(level)
            {
                case BondingLevels.Awakened:
                    return Color.Gold;
                case BondingLevels.Loyal:
                    return Color.Pink;
                case BondingLevels.Companion:
                    return Color.MediumAquamarine;
                case BondingLevels.Inquisitive:
                    return Color.SkyBlue;
                case BondingLevels.Relaxed:
                    return Color.LightSalmon;
                case BondingLevels.Tolerant:
                    return Color.PaleGreen;
                case BondingLevels.Wary:
                    return Color.Tan;
                default:
                    return Color.LightGray;
            }
            //return Color.LightGray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
