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

    public int CurrentInventoryPlusOrdered { get; set; }
    public bool ButtonIsVisible { get; set; }
    public Color CurrentInventoryColor { get; set; }

}

public class AccountsPayable
{
    public string AccountsPayableID { get; set; }
    public double Amount { get; set; }
    public string DueDate { get; set; }
    public string PaymentStatus { get; set; }
    public bool ButtonIsVisible { get; set; }
    public Color PastDueColor { get; set; }

}

public class AccountsPayableVendor : AccountsPayable
{
    public string RawMaterialID { get; set; }
    public int RawMaterialQty { get; set; }
    public string VendorID { get; set; }
}

public class AccountsPayableEmployee : AccountsPayable
{
    public string EmployeeId { get; set; }
    public bool EmployeeDirectDeposit { get; set; }
}

public class TransactionPayable : Transaction
{
    public string AccountsPayableID { get; set; }

}

