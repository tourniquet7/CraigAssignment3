// App:     Amortization calculator
// Element: ViewModel

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FISSystem.Models;

namespace FISSystem.ViewModels;

public partial class ARViewModel : ObservableObject
{
    [ObservableProperty]
    private bool upcomingPaymentsVisible = true;

    [ObservableProperty]
    private bool transactionsVisible = false;



    [ObservableProperty]
    private Color upcomingPaymentsButtonColor = Colors.Green;

    [ObservableProperty]
    private Color transactionsButtonColor = Colors.LightGray;




    public ARViewModel()
    {
        
    }

    private void ARInvisible()
    {
        UpcomingPaymentsVisible = false;
        UpcomingPaymentsButtonColor = Colors.LightGray;
        TransactionsVisible = false;
        TransactionsButtonColor = Colors.LightGray;
        
    }

    [RelayCommand]
    private void ARViewSelected(string view)
    {
        ARInvisible();
        if (view == "upcomingPayments")
        {
            UpcomingPaymentsVisible = true;
            UpcomingPaymentsButtonColor = Colors.Green;
        } 
        else if (view == "transactions")
        {
            TransactionsVisible = true;
            TransactionsButtonColor = Colors.Green;
        } 
    }
}
