using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace TicTacToe.Converters
{
    /// <summary>
    /// Converter that converts a 'false' value to Visibility.Visible and a 'true' value to to Visibility.Collapsed.
    /// </summary>
    public class InverseBooleanToVisibilityCollapsedConverter : IValueConverter 
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            if (value is bool booleanValue)
            { 
                return booleanValue ? Visibility.Collapsed : Visibility.Visible; 
            } 

            return Visibility.Visible;
        }

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            if (value is Visibility visibility) 
            {
                return visibility == Visibility.Collapsed;
            } 

            return false;
        } 
    }
}
