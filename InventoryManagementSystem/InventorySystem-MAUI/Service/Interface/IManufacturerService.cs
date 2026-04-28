using System.Text.Json;
using InventorySystem_Shared.Inventory.Manufacturer;

namespace InventorySystem_MAUI.Service;

public interface IManufacturerService
{

    Task<List<InventoryManufacturer>> GetManufacturersAsync();

    Task SaveManufacturersAsync(List<InventoryManufacturer> manufacturers);

    Task UpsertManufacturerAsync(InventoryManufacturer manufacturer, string? oldName = null);
}