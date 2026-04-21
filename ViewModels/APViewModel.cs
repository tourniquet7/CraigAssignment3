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
    private bool vendorVisible;

    [ObservableProperty]
    private bool employeeVisible;

    [ObservableProperty]
    private bool transactionsVisible;

    [ObservableProperty]
    private string vendorButtonColor;

    [ObservableProperty]
    private string employeeButtonColor;

    [ObservableProperty]
    private string transactionsButtonColor;


    public ObservableCollection<ModelAccountsReceivable> Schedule { get; } = new();

    public APViewModel()
    {
        APInvisible();
        vendorVisible = true;
        vendorButtonColor = "Green";
    }

    private void APInvisible()
    {
        transactionsVisible = false;
        transactionsButtonColor = "LightGray";
        vendorVisible = false;
        vendorButtonColor = "LightGray";
        employeeVisible = false;
        employeeButtonColor = "LightGray";
    }

    [RelayCommand]
    private void APViewSelected(string view)
    {
        APInvisible();
        if (view == "vendor")
        {
            vendorVisible = true;
            vendorButtonColor = "Green";
        } 
        else if (view == "employee")
        {
            employeeVisible = true;
            employeeButtonColor = "Green";
        } 
        else if (view == "transactions")
        {
            transactionsVisible = true;
            transactionsButtonColor = "Green";
        }
    }
}
