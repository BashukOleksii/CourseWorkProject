using InventorySystem_API.Inventory.Models;
using System.Globalization;

namespace InventorySystem_MAUI.Converters;

public class InventoryTypeToStringConverter : IValueConverter
{
    private static readonly Dictionary<InventoryType, string> _translations = new()
    {
        { InventoryType.Electronics, "Електроніка" },
        { InventoryType.Clothing, "Одяг" },
        { InventoryType.Food, "Продукти харчування" },
        { InventoryType.Household, "Побутові товари" },
        { InventoryType.HealthAndBeauty, "Здоров'я та краса" },
        { InventoryType.Sports, "Спорт" },
        { InventoryType.Children, "Дитячі товари" },
        { InventoryType.Office, "Канцтовари" },
        { InventoryType.Auto, "Автотовари" },
        { InventoryType.Other, "Інше" }
    };

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is InventoryType type && _translations.TryGetValue(type, out var text))
            return text;
        return "Невідомо";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}