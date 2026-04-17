using System.Text.Json;
using InventorySystem_Shared.Inventory.Manufacturer;

namespace InventorySystem_MAUI.Service;

public class ManufacturerService
{
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "manufacturers.json");

    public async Task<List<InventoryManufacturer>> GetManufacturersAsync()
    {
        if (!File.Exists(_filePath))
            return new List<InventoryManufacturer>();

        using var stream = File.OpenRead(_filePath);
        if (stream.Length == 0) return new List<InventoryManufacturer>();

        return await JsonSerializer.DeserializeAsync<List<InventoryManufacturer>>(stream)
               ?? new List<InventoryManufacturer>();
    }

    public async Task SaveManufacturersAsync(List<InventoryManufacturer> manufacturers)
    {
        using var stream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(stream, manufacturers, new JsonSerializerOptions { WriteIndented = true });
    }

    public async Task UpsertManufacturerAsync(InventoryManufacturer manufacturer, string? oldName = null)
    {
        var list = await GetManufacturersAsync();

        if (!string.IsNullOrEmpty(oldName))
        {
            var existing = list.FirstOrDefault(m => m.Name.Equals(oldName, StringComparison.OrdinalIgnoreCase));
            if (existing != null) list.Remove(existing);
        }

        list.Add(manufacturer);
        await SaveManufacturersAsync(list);
    }
}