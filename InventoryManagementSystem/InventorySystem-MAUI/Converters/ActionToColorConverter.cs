using System.Globalization;
using InventorySystem_Shared.Loging;

namespace InventorySystem_MAUI.Converters
{
    public class ActionToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ActionType action)
            {
                return action switch
                {
                    ActionType.Create or ActionType.CreateMany => Color.FromArgb("#22C55E"), 
                    ActionType.Delete => Color.FromArgb("#EF4444"), 
                    ActionType.Update => Color.FromArgb("#F59E0B"), 
                    _ => Color.FromArgb("#6366F1") 
                };
            }
            return Colors.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}