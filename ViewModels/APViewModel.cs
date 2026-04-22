// App:     Amortization calculator
// Element: ViewModel

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FISSystem.Models;
using FISSystem.Services;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Json.Nodes;
using System.Windows.Input;

namespace FISSystem.ViewModels;

public partial class APViewModel : ObservableObject
{

    FisMySqlHelper mysqlHelper = new FisMySqlHelper();

    [ObservableProperty]
    private bool rawVisible = true;

    [ObservableProperty]
    private bool vendorVisible = false;

    [ObservableProperty]
    private bool employeeVisible = false;

    [ObservableProperty]
    private bool transactionsVisible = false;

    [ObservableProperty]
    private Color rawButtonColor = Colors.Green;

    [ObservableProperty]
    private Color vendorButtonColor = Colors.LightGray;

    [ObservableProperty]
    private Color employeeButtonColor = Colors.LightGray;

    [ObservableProperty]
    private Color transactionsButtonColor = Colors.LightGray;

    public int width = 200;

    [ObservableProperty]
    DataTable raw_tbl = new DataTable("raw");

    public ObservableCollection<RawMaterial> RawMaterials { get; } = new();

    public ObservableCollection<ModelAccountsReceivable> Schedule { get; } = new();


    public ICommand OrderMaterial
    {
        get;
    }

    public APViewModel()
    {
        
        ShowRawMaterials();
        ShowTransactions();
        OrderMaterial = new Command<string>(OrderMaterialFunction);
    }


    private void OrderMaterialFunction(string rawId)
    {
        var rawMaterial = mysqlHelper.GetRawMaterial(rawId);
        if (rawMaterial == null) return;
        int currentInventory = ((int)rawMaterial["CurrentInventory"]);
        int lowInventoryLevel = ((int)rawMaterial["LowInventoryLevel"]);
        int inventoryReplenishLevel = ((int)rawMaterial["InventoryReplenishLevel"]);


        if (currentInventory < lowInventoryLevel)
        {
            int numberToOrder = inventoryReplenishLevel - currentInventory;
            mysqlHelper.CreateRawMaterialTransaction(rawId, numberToOrder);
            mysqlHelper.UpdateRawMaterialAfterOrder(rawId, numberToOrder);

            ShowRawMaterials();
        }
    }

    private void ShowRawMaterials()
    {
        var response = mysqlHelper.GetRawMaterials();
        if (response == null || response.Count == 0)
            return;

        RawMaterials.Clear();

        foreach (var rawMaterial in response.AsArray())
        {
            
            

            var rawMaterialId = rawMaterial["RawMaterialID"]?.ToString();
            var preferredVendorId = rawMaterial["PreferredVendorID"]?.ToString();
            var name = rawMaterial["Name"]?.ToString();
            var unitOfMeasurement = rawMaterial["UnitOfMeasurement"]?.ToString();
            var currentInventory = ((int)rawMaterial["CurrentInventory"]);
            var lowInventoryLevel = ((int)rawMaterial["LowInventoryLevel"]);
            var inventoryReplenishLevel = ((int)rawMaterial["InventoryReplenishLevel"]);

            var isVisible = false;
            var inventoryTextColor = Colors.Gray;

            if (currentInventory < lowInventoryLevel)
            {
                isVisible = true;
                inventoryTextColor = Colors.Red;
            }


            RawMaterials.Add(new RawMaterial { RawMaterialId = rawMaterialId, PreferredVendorId = preferredVendorId, Name = name, UnitOfMeasurement = unitOfMeasurement, CurrentInventory = currentInventory, LowInventoryLevel = lowInventoryLevel, InventoryReplenishLevel = inventoryReplenishLevel, ButtonIsVisible = isVisible, CurrentInventoryColor = inventoryTextColor});
        }

    }

    private void ShowTransactions()
    {
        var response = mysqlHelper.GetRawMaterials();
        if (response == null || response.Count == 0)
            return;

        RawMaterials.Clear();

        foreach (var rawMaterial in response.AsArray())
        {



            var rawMaterialId = rawMaterial["RawMaterialID"]?.ToString();
            var preferredVendorId = rawMaterial["PreferredVendorID"]?.ToString();
            var name = rawMaterial["Name"]?.ToString();
            var unitOfMeasurement = rawMaterial["UnitOfMeasurement"]?.ToString();
            var currentInventory = ((int)rawMaterial["CurrentInventory"]);
            var lowInventoryLevel = ((int)rawMaterial["LowInventoryLevel"]);
            var inventoryReplenishLevel = ((int)rawMaterial["InventoryReplenishLevel"]);

            var isVisible = false;
            var inventoryTextColor = Colors.Gray;

            if (currentInventory < lowInventoryLevel)
            {
                isVisible = true;
                inventoryTextColor = Colors.Red;
            }


            RawMaterials.Add(new RawMaterial { RawMaterialId = rawMaterialId, PreferredVendorId = preferredVendorId, Name = name, UnitOfMeasurement = unitOfMeasurement, CurrentInventory = currentInventory, LowInventoryLevel = lowInventoryLevel, InventoryReplenishLevel = inventoryReplenishLevel, ButtonIsVisible = isVisible, CurrentInventoryColor = inventoryTextColor });
        }

    }

    private void APInvisible()
    {
        RawVisible = false;
        RawButtonColor = Colors.LightGray;
        TransactionsVisible = false;
        TransactionsButtonColor = Colors.LightGray;
        VendorVisible = false;
        VendorButtonColor = Colors.LightGray;
        EmployeeVisible = false;
        EmployeeButtonColor = Colors.LightGray;
    }

    [RelayCommand]
    private void APViewSelected(string view)
    {
        APInvisible();

        if (view == "raw")
        {
            RawVisible = true;
            RawButtonColor = Colors.Green;
        }
        else if (view == "vendor")
        {
            VendorVisible = true;
            VendorButtonColor = Colors.Green;
        } 
        else if (view == "employee")
        {
            EmployeeVisible = true;
            EmployeeButtonColor = Colors.Green;
        } 
        else if (view == "transactions")
        {
            TransactionsVisible = true;
            TransactionsButtonColor = Colors.Green;
        }
    }
}
