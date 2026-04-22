// App:     Amortization calculator
// Element: Model

namespace FISSystem.Models;

public class RawMaterial
{
    public string RawMaterialId { get; set; }
    public string PreferredVendorId { get; set; }
    public string Name { get; set; }
    public string UnitOfMeasurement { get; set; }
    public int CurrentInventory { get; set; }
    public int LowInventoryLevel { get; set; }
    public int InventoryReplenishLevel { get; set; }

}

