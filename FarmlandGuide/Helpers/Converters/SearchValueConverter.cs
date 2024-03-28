using System;
using System.Linq;
using System.Windows.Data;

namespace FarmlandGuide.Helpers.Converters
{
    public class SearchValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string searchText = values.Last() as string;
            if (string.IsNullOrEmpty(searchText)) return true;
            bool res=false;
            for (int i = 0; i < values.Length - 1; i++)
            {
                var cellText = values[i] == null ? string.Empty : values[i].ToString();
                if (!string.IsNullOrEmpty(searchText) && !string.IsNullOrEmpty(cellText))
                    res |= cellText.ToLower().Contains(searchText.ToLower());
            }
            return res;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
