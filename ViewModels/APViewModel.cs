// App:     Amortization calculator
// Element: ViewModel

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FISSystem.Models;
using FISSystem.Pages;
using FISSystem.Services;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Windows.Input;

namespace FISSystem.ViewModels;

public partial class APViewModel : ObservableObject
{

    FisMySqlHelperAccountsPayable mysqlHelper = new FisMySqlHelperAccountsPayable();

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


    public ObservableCollection<RawMaterial> RawMaterials { get; } = new();
    public ObservableCollection<AccountsPayableVendor> AccountsPayableVendor { get; } = new();
    public ObservableCollection<TransactionPayable> TransactionPayable { get; } = new();

    public ObservableCollection<AccountsPayableEmployee> AccountsPayableEmployee { get; } = new();

    



    public ICommand OrderMaterial
    {
        get;
    }

    public ICommand VendorTransaction
    {
        get;
    }

    public ICommand EmployeeTransaction
    {
        get;
    }

    public APViewModel()
    {
        
        
        OrderMaterial = new Command<string>(OrderMaterialFunction);
        VendorTransaction = new Command<string>(VendorTransactionFunction);
        EmployeeTransaction = new Command<string>(EmployeeTransactionFunction);
        RefreshAccountsPayable();
    }

    private void RefreshAccountsPayable()
    {
        ShowRawMaterials();
        ShowAccountsPayableVendor();
        ShowTransactions();
        ShowAccountsPayableEmployee();
    }

    private void OrderMaterialFunction(string rawId)
    {
        var rawMaterial = mysqlHelper.GetRawMaterial(rawId);
        if (rawMaterial == null) return;
        int currentInventoryPlusOrdered = ((int)rawMaterial["CurrentInventoryPlusOrdered"]);
        int lowInventoryLevel = ((int)rawMaterial["LowInventoryLevel"]);
        int inventoryReplenishLevel = ((int)rawMaterial["InventoryReplenishLevel"]);


        if (currentInventoryPlusOrdered < lowInventoryLevel)
        {
            int numberToOrder = inventoryReplenishLevel - currentInventoryPlusOrdered;
            mysqlHelper.CreateRawMaterialTransaction(rawId, numberToOrder);
            mysqlHelper.UpdateRawMaterialAfterOrder(rawId, numberToOrder);

            RefreshAccountsPayable();
        }
    }

    private void VendorTransactionFunction(string id)
    {
        var accountPayable = mysqlHelper.GetAccountPayable(id);
        if (accountPayable == null) return;
        string accountsPayableID = (accountPayable["AccountsPayableID"]?.ToString());
        string vendorID = (accountPayable["VendorID"]?.ToString());
        double amount = double.Parse(accountPayable["Amount"]?.ToString() ?? "0"); 
        
        mysqlHelper.CreateAccountsPayableTransaction(accountsPayableID, vendorID, amount, "vendor");
        mysqlHelper.UpdateAccountsPayableAfterTransaction(accountsPayableID);

        RefreshAccountsPayable();


    }

    private void EmployeeTransactionFunction(string id)
    {
        var accountPayable = mysqlHelper.GetAccountPayable(id);
        if (accountPayable == null) return;
        string accountsPayableID = (accountPayable["AccountsPayableID"]?.ToString());
        string employeeId = (accountPayable["EmployeeId"]?.ToString());
        double amount = double.Parse(accountPayable["Amount"]?.ToString() ?? "0");

        mysqlHelper.CreateAccountsPayableTransaction(accountsPayableID, employeeId, amount, "employee");
        mysqlHelper.UpdateAccountsPayableAfterTransaction(accountsPayableID);

        RefreshAccountsPayable();
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
            var currentInventoryPlusOrdered = ((int)rawMaterial["CurrentInventoryPlusOrdered"]);

            var isVisible = false;
            var inventoryTextColor = Colors.Gray;

            if (currentInventoryPlusOrdered < lowInventoryLevel)
            {
                isVisible = true;
                inventoryTextColor = Colors.Red;
            }


            RawMaterials.Add(new RawMaterial { RawMaterialId = rawMaterialId, 
                PreferredVendorId = preferredVendorId, 
                Name = name, 
                UnitOfMeasurement = unitOfMeasurement, 
                CurrentInventory = currentInventory, 
                LowInventoryLevel = lowInventoryLevel, 
                InventoryReplenishLevel = inventoryReplenishLevel, 
                ButtonIsVisible = isVisible, 
                CurrentInventoryColor = inventoryTextColor,
                CurrentInventoryPlusOrdered = currentInventoryPlusOrdered
            });
        }

    }

    private void ShowAccountsPayableVendor()
    {
        var response = mysqlHelper.GetAccountsPayableVendor();
        if (response == null || response.Count == 0)
            return;

        AccountsPayableVendor.Clear();

        foreach (var accountPayable in response.AsArray())
        {
            

            string accountsPayableID = (accountPayable["AccountsPayableID"]?.ToString()?.Trim('"'));
            double amount = double.Parse(accountPayable["Amount"]?.ToString()?.Trim('"') ?? "0");
            DateTime dueDate = ((DateTime)accountPayable["DueDate"]);
            string paymentStatus = accountPayable["PaymentStatus"]?.ToString()?.Trim('"');
            string rawMaterialID = accountPayable["RawMaterialID"]?.ToString()?.Trim('"');
            int rawMaterialQty = ((int)accountPayable["RawMaterialQty"]);
            string vendorID = accountPayable["VendorID"]?.ToString()?.Trim('"');


            bool isVisible = false;
            Color pastDueColor = Colors.Gray;
            DateTime today = DateTime.Now;

            string dueDateString = dueDate.ToShortDateString();

            if (today >= dueDate && paymentStatus == "Pending")
            {
                isVisible = true;
                pastDueColor = Colors.Red;
            }


            AccountsPayableVendor.Add(new AccountsPayableVendor
            {
                AccountsPayableID = accountsPayableID,
                Amount = amount,
                DueDate = dueDateString,
                PaymentStatus = paymentStatus,
                RawMaterialID = rawMaterialID,
                RawMaterialQty = rawMaterialQty,
                VendorID = vendorID,
                ButtonIsVisible = isVisible,
                PastDueColor = pastDueColor
            });
        }

    }

    private void ShowAccountsPayableEmployee()
    {
        var response = mysqlHelper.GetAccountsPayableEmployee();
        if (response == null || response.Count == 0)
            return;

        AccountsPayableEmployee.Clear();

        foreach (var accountPayable in response.AsArray())
        {


            string accountsPayableID = (accountPayable["AccountsPayableID"]?.ToString()?.Trim('"'));
            double amount = double.Parse(accountPayable["Amount"]?.ToString()?.Trim('"') ?? "0");
            DateTime dueDate = ((DateTime)accountPayable["DueDate"]);
            string paymentStatus = accountPayable["PaymentStatus"]?.ToString()?.Trim('"');
            string employeeId = accountPayable["EmployeeId"]?.ToString()?.Trim('"');
            bool employeeDirectDeposit = accountPayable["EmployeeDirectDeposit"]?.GetValue<bool>() ?? false;



            bool isVisible = false;
            Color pastDueColor = Colors.Gray;
            DateTime today = DateTime.Now;

            string dueDateString = dueDate.ToShortDateString();

            if (today >= dueDate && paymentStatus == "Pending")
            {
                isVisible = true;
                pastDueColor = Colors.Red;
            }


            AccountsPayableEmployee.Add(new AccountsPayableEmployee
            {
                AccountsPayableID = accountsPayableID,
                Amount = amount,
                DueDate = dueDateString,
                PaymentStatus = paymentStatus,
                EmployeeId = employeeId,
                EmployeeDirectDeposit = employeeDirectDeposit,
                ButtonIsVisible = isVisible,
                PastDueColor = pastDueColor
            });
        }

    }

    private void ShowTransactions()
    {
        var response = mysqlHelper.GetAccountsPayableTransactions();
        if (response == null || response.Count == 0)
            return;

        TransactionPayable.Clear();

        foreach (var transactions in response.AsArray())
        {
            var transactionID = transactions["TransactionID"]?.ToString();
            var accountsPayableID = transactions["AccountsPayableID"]?.ToString();
            double amount = double.Parse(transactions["Amount"]?.ToString() ?? "0");
            DateTime date = ((DateTime)transactions["Date"]);
            var employeeId = transactions["EmployeeId"]?.ToString();
            var vendorID = transactions["VendorID"]?.ToString();

            TransactionPayable.Add(new TransactionPayable
            {
                TransactionID = transactionID, 
                AccountsPayableID = accountsPayableID, 
                Amount = amount, 
                Date = date
            });
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
