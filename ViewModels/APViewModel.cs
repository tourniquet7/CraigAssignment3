// App:     Amortization calculator
// Element: ViewModel

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FISSystem.Models;
using FISSystem.Services;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Data;

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

    public APViewModel()
    {
        PopulateRawMaterials();
    }

    private void PopulateRawMaterials()
    {

        var response = mysqlHelper.GetRawMaterials();
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
