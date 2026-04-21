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
    private string loanAmountText;

    [ObservableProperty]
    private string annualRateText;

    [ObservableProperty]
    private string loanYearsText;

    [ObservableProperty]
    private decimal monthlyPayment;

    public ObservableCollection<ModelAccountsReceivable> Schedule { get; } = new();

    public APViewModel()
    {
    }

    [RelayCommand]
    private void GenerateSchedule()
    {
        if (!decimal.TryParse(loanAmountText, out var loanAmount) || loanAmount <= 0) return;
        if (!decimal.TryParse(annualRateText, out var annualRate) || annualRate <= 0) return;
        if (!int.TryParse(loanYearsText, out var loanYears) || loanYears <= 0) return;

        Schedule.Clear();

        int numPayments = loanYears * 12;
        decimal monthlyRate = annualRate / 100 / 12;

        MonthlyPayment = loanAmount *
            (monthlyRate * (decimal)Math.Pow(1 + (double)monthlyRate, numPayments)) /
            ((decimal)Math.Pow(1 + (double)monthlyRate, numPayments) - 1);

        decimal balance = loanAmount;

        for (int i = 1; i <= numPayments; i++)
        {
        }
    }
}
