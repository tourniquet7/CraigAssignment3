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

public partial class ReportsViewModel : ObservableObject
{

    FisMySqlHelperReports mysqlHelper = new FisMySqlHelperReports();

  

    [ObservableProperty]
    private bool accountsPayableVisible = true;

    [ObservableProperty]
    private bool accountsReceivableVisible = false;

    [ObservableProperty]
    private bool transactionsVisible = false;


   

    [ObservableProperty]
    private Color accountsPayableColor = Colors.Green;

    [ObservableProperty]
    private Color accountsReceivableColor = Colors.LightGray;

    [ObservableProperty]
    private Color transactionsColor = Colors.LightGray;

    public int width = 200;


    public ObservableCollection<AccountsPayableEverything> AccountsPayableEverything { get; } = new();

    public ObservableCollection<AccountsReceivableModel> AccountsReceivableModel { get; } = new();

    public ICommand RefreshReportsButton
    {
        get;
    }


    public ReportsViewModel()
    {
        RefreshReportsButton = new Command<string>(RefreshReportsButtonFunction);
        RefreshReports();
    }

    private void RefreshReportsButtonFunction(string parameter)
    {
        RefreshReports();
    }

    private void RefreshReports()
    {
        ShowAccountsPayable();
        ShowAccountsReceivable();
    }

    private void ShowAccountsPayable()
    {
        var response = mysqlHelper.GetAccountsPayable();
        if (response == null || response.Count == 0)
            return;

        AccountsPayableEverything.Clear();

        double amountPaid = 0;
        double amountPastDue = 0;
        double amountTotal = 0;

        foreach (var accountPayable in response.AsArray())
        {


            string accountsPayableID = (accountPayable["AccountsPayableID"]?.ToString()?.Trim('"'));
            string rawMaterialID = (accountPayable["RawMaterialID"]?.ToString()?.Trim('"'));
            int rawMaterialQty = int.Parse(accountPayable["RawMaterialQty"]?.ToString()?.Trim('"') ?? "0");
            string vendorID = (accountPayable["VendorID"]?.ToString()?.Trim('"'));
            string employeeId = (accountPayable["EmployeeId"]?.ToString()?.Trim('"'));
            double  amount = double.Parse(accountPayable["Amount"]?.ToString()?.Trim('"') ?? "0");
            DateTime dueDate = ((DateTime)accountPayable["DueDate"]);
            string paymentStatus = accountPayable["PaymentStatus"]?.ToString()?.Trim('"');
            bool employeeDirectDeposit = (accountPayable["EmployeeDirectDeposit"] != null) ? (bool)accountPayable["EmployeeDirectDeposit"] : false;

            Color pastDueColor = Colors.Gray;

            string dueDateString = dueDate.ToShortDateString();

            AccountsPayableEverything.Add(new AccountsPayableEverything
            {
                AccountsPayableID = accountsPayableID,
                RawMaterialID = rawMaterialID,
                RawMaterialQty = rawMaterialQty,
                VendorID = vendorID,
                EmployeeId = employeeId,
                Amount = amount,
                DueDate = dueDateString,
                PaymentStatus = paymentStatus,
                PastDueColor = pastDueColor
            });
        }
    }


    private void ShowAccountsReceivable()
    {
        var response = mysqlHelper.GetAccountsReceivable();
        if (response == null || response.Count == 0)
            return;

        AccountsPayableEverything.Clear();

        double amountPaid = 0;
        double amountPastDue = 0;
        double amountTotal = 0;

        foreach (var accountReceivable in response.AsArray())
        {


            string accountsReceivableID = (accountReceivable["AccountsReceivableID"]?.ToString()?.Trim('"'));
            string customerID = (accountReceivable["CustomerID"]?.ToString()?.Trim('"'));
            double amount = double.Parse(accountReceivable["Amount"]?.ToString()?.Trim('"') ?? "0");
            DateTime dueDate = ((DateTime)accountReceivable["DueDate"]);
            string paymentStatus = accountReceivable["PaymentStatus"]?.ToString()?.Trim('"');


            AccountsReceivableModel.Add(new AccountsReceivableModel
            {
                AccountsReceivableID = accountsReceivableID,
                CustomerID = customerID,
                Amount = amount,
                DueDate = dueDate,
                PaymentStatus = paymentStatus
            });
        }
    }






    private void ReportsInvisible()
    {
        AccountsReceivableVisible = false;
        AccountsReceivableColor = Colors.LightGray;
        TransactionsVisible = false;
        TransactionsColor = Colors.LightGray;
        AccountsPayableVisible = false;
        AccountsPayableColor = Colors.LightGray;
    }

    [RelayCommand]
    private void ReportsViewSelected(string view)
    {
        ReportsInvisible();

        if (view == "accountsReceivable")
        {
            AccountsReceivableVisible = true;
            AccountsReceivableColor = Colors.Green;
        }
        else if (view == "accountsPayable")
        {
            AccountsPayableVisible = true;
            AccountsPayableColor = Colors.Green;
        } 
        else if (view == "transactions")
        {
            TransactionsVisible = true;
            TransactionsColor = Colors.Green;
        } 
    }
}
