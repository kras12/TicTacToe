using System.Globalization;
using System.Windows.Data;

namespace TicTacToe.Converters;

/// <summary>
/// Converter that converts a height in double to a font size value by applying a scale to the height. 
/// </summary>
public class HeightToFontSizeConverter : IValueConverter
{
    /// <inheritdoc/>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double actualHeight && parameter is string parameterString 
            && double.TryParse(parameterString, NumberStyles.Any, CultureInfo.InvariantCulture, out double scale))
        {
            return actualHeight * scale;
        }

        throw new ArgumentException("The value and parameter must be of type double");
    }

    /// <inheritdoc/>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
