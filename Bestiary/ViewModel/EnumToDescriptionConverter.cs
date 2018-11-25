using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Bestiary.ViewModel
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        private string GetEnumDescription(Enum enumObject)
        {
            FieldInfo fieldInfo = enumObject.GetType().GetField(enumObject.ToString());
            object[] attributes = fieldInfo.GetCustomAttributes(false);
            if(attributes.Length == 0)
            {
                return enumObject.ToString();
            }
            else
            {
                DescriptionAttribute attribute = attributes[0] as DescriptionAttribute;
                return attribute.Description;
            }
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Enum enumForConversion = (Enum)value;
            string desc = GetEnumDescription(enumForConversion);
            return desc;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
