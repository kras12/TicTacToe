
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TicTacToe.Converters;

// TODO - This converter will likely not be used now.
public class EnumToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value != null && parameter != null)
        {
            if (value.Equals(parameter))
            {
                return Visibility.Visible;
            }
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
