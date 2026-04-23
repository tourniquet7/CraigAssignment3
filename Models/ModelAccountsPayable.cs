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

    public int CurrentInventoryPlusOrdered { get; set; }
    public bool ButtonIsVisible { get; set; }
    public Color CurrentInventoryColor { get; set; }

}

public class AccountsPayableVendor : AccountsPayableModel
{
    public string RawMaterialID { get; set; }
    public int RawMaterialQty { get; set; }
    public string VendorID { get; set; }
}

public class AccountsPayableEmployee : AccountsPayableModel
{
    public string EmployeeId { get; set; }
    public bool EmployeeDirectDeposit { get; set; }
}

public class TransactionPayable : Transaction
{
    public string AccountsPayableID { get; set; }

}

