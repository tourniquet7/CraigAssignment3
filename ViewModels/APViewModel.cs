// App:     Amortization calculator
// Element: ViewModel

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FISSystem.Models;

namespace FISSystem.ViewModels;

public partial class APViewModel : ObservableObject
{
    [ObservableProperty]
    private bool vendorVisible = true;

    [ObservableProperty]
    private bool employeeVisible = false;

    [ObservableProperty]
    private bool transactionsVisible = false;

    [ObservableProperty]
    private Color vendorButtonColor = Colors.Green;

    [ObservableProperty]
    private Color employeeButtonColor = Colors.LightGray;

    [ObservableProperty]
    private Color transactionsButtonColor = Colors.LightGray;


    public ObservableCollection<ModelAccountsReceivable> Schedule { get; } = new();

    public APViewModel()
    {
        
    }

    private void APInvisible()
    {
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
        if (view == "vendor")
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
