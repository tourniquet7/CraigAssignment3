// App:     Amortization calculator
// Element: ViewModel

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FISSystem.Models;
using FISSystem.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace FISSystem.ViewModels;

public partial class ARViewModel : ObservableObject
{

    FisMySqlHelperAccountsReceivable mysqlHelper = new FisMySqlHelperAccountsReceivable();

    [ObservableProperty]
    private bool upcomingPaymentsVisible = true;

    [ObservableProperty]
    private bool transactionsVisible = false;

    [ObservableProperty]
    private Color upcomingPaymentsButtonColor = Colors.Green;

    [ObservableProperty]
    private Color transactionsButtonColor = Colors.LightGray;

    public ObservableCollection<AccountsReceivableModel> AccountsReceivableModel { get; } = new();

    public ObservableCollection<TransactionReceivable> TransactionReceivable { get; } = new();

    public ICommand SimulateOrder
    {
        get;
    }



    public ARViewModel()
    {
        SimulateOrder = new Command<string>(SimulateOrderFunction);
        RefreshAccountsReceivable();
    }

    private void SimulateOrderFunction(string accountsReceivableID)
    {
        mysqlHelper.CreateCustomerPayment(accountsReceivableID);
        RefreshAccountsReceivable();
    }

    private void RefreshAccountsReceivable()
    {
        ShowAccountsReceivable();
        ShowTransactions();
    }

    private void ShowAccountsReceivable()
    {
        var response = mysqlHelper.GetAccountsReceivable();
        if (response == null || response.Count == 0)
            return;

        AccountsReceivableModel.Clear();

        foreach (var accountsReceivable in response.AsArray())
        {


            string accountsReceivableID = (accountsReceivable["AccountsReceivableID"]?.ToString()?.Trim('"'));
            string customerID = (accountsReceivable["CustomerID"]?.ToString()?.Trim('"'));
            double amount = double.Parse(accountsReceivable["Amount"]?.ToString()?.Trim('"') ?? "0");

            DateTime dueDate = ((DateTime)accountsReceivable["DueDate"]);
            string paymentStatus = accountsReceivable["PaymentStatus"]?.ToString()?.Trim('"');


     
            Color pastDueColor = Colors.Gray;
           

            string dueDateString = dueDate.ToShortDateString();
            bool isVisible = false;

            if (paymentStatus == "Pending")
            {
                isVisible = true;
                pastDueColor = Colors.Red;
            }


            AccountsReceivableModel.Add(new AccountsReceivableModel
            {
                AccountsReceivableID = accountsReceivableID,
                CustomerID = customerID,
                Amount = amount,
                DueDate = dueDate,
                PaymentStatus = paymentStatus,
                ButtonIsVisible = isVisible,
                PastDueColor = pastDueColor
            });
        }

    }

    private void ShowTransactions()
    {
        var response = mysqlHelper.GetAccountsReceivableTransactions();
        if (response == null || response.Count == 0)
            return;

        TransactionReceivable.Clear();
        foreach (var transactions in response.AsArray())
        {
            var transactionID = transactions["TransactionID"]?.ToString();
            var accountsReceivableID = transactions["AccountsReceivableID"]?.ToString();
            double amount = double.Parse(transactions["Amount"]?.ToString() ?? "0");
            DateTime date = ((DateTime)transactions["Date"]);
        

            TransactionReceivable.Add(new TransactionReceivable
            {
                TransactionID = transactionID,
                AccountsReceivableID = accountsReceivableID,
                Amount = amount,
                Date = date
            });
        }
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
