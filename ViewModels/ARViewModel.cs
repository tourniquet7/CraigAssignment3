// App:     Amortization calculator
// Element: ViewModel

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FISSystem.Models;
using FISSystem.Services;
using System.Collections.ObjectModel;

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

    public ObservableCollection<AccountsReceivable> AccountsReceivable { get; } = new();


    public ARViewModel()
    {
        RefreshAccountsReceivable();
    }

    private void RefreshAccountsReceivable()
    {
        ShowAccountsPayableVendor();
        ShowAccountsPayableTransactions();
    }

    private void ShowAccountsPayableTransactions()
    {
        var response = mysqlHelper.GetAccountsReceivable();
        if (response == null || response.Count == 0)
            return;

        AccountsReceivable.Clear();

        foreach (var accountReceivable in response.AsArray())
        {


            string accountsReceivableID = (accountReceivable["AccountsReceivableID"]?.ToString()?.Trim('"'));
            string customerID = (accountReceivable["CustomerID"]?.ToString()?.Trim('"'));
            double amount = double.Parse(accountReceivable["Amount"]?.ToString()?.Trim('"') ?? "0");
            DateTime dueDate = ((DateTime)accountReceivable["DueDate"]);
            string paymentStatus = accountReceivable["PaymentStatus"]?.ToString()?.Trim('"');



            bool isVisible = false;
            Color pastDueColor = Colors.Gray;
            DateTime today = DateTime.Now;

            string dueDateString = dueDate.ToShortDateString();

            if (today >= dueDate && paymentStatus == "Pending")
            {
                isVisible = true;
                pastDueColor = Colors.Red;
            }


            AccountsReceivable.Add(new AccountsReceivable
            {
                AccountsReceivableID = accountsReceivableID,
                CustomerID = customerID,
                Amount = amount,
                DueDate = dueDate,
                PaymentStatus = paymentStatus
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
