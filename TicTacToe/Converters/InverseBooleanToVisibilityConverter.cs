using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace TicTacToe.Converters
{
    public class InverseBooleanToVisibilityConverter : IValueConverter 
    { 
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        { 
            if (value is bool booleanValue)
            { 
                return booleanValue ? Visibility.Collapsed : Visibility.Visible; 
            } 

            return Visibility.Visible;
        } 
        
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
