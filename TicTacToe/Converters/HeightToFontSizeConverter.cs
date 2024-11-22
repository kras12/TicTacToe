using System.Globalization;
using System.Windows.Data;

namespace TicTacToe.Converters;

public class HeightToFontSizeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double actualHeight && parameter is string parameterString 
            && double.TryParse(parameterString, NumberStyles.Any, CultureInfo.InvariantCulture, out double scale))
        {
            return actualHeight * scale;
        }

        throw new ArgumentException("The value and parameter must be of type double");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
