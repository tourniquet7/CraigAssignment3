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

    [ObservableProperty]
    private double apAmountPaid = 0;

    [ObservableProperty]
    private double apAmountNotPaid = 0;

    [ObservableProperty]
    private double apAmountPastDue = 0;

    [ObservableProperty]
    private double apAmountTotal = 0;

    [ObservableProperty]
    private int apAccountsPastDue = 0;

    [ObservableProperty]
    private double arAmountPaid = 0;

    [ObservableProperty]
    private double arAmountNotPaid = 0;

    [ObservableProperty]
    private double arAmountPastDue = 0;

    [ObservableProperty]
    private double arAmountTotal = 0;

    [ObservableProperty]
    private int arAccountsPastDue = 0;

    [ObservableProperty]
    private double tAmountPastDue = 0;

    [ObservableProperty]
    private double trTotalPaid = 0;

    [ObservableProperty]
    private double trTotalReceived = 0;

    [ObservableProperty]
    private double trRevenueMinusExpenses = 0;


    public int width = 200;


    public ObservableCollection<AccountsPayableEverything> AccountsPayableEverything { get; } = new();

    public ObservableCollection<AccountsReceivableModel> AccountsReceivableModel { get; } = new();

    public ObservableCollection<TransactionEverything> TransactionEverything { get; } = new();



    public ReportsViewModel()
    {
       
        RefreshReports();
    }



    [RelayCommand]
    private void RefreshReports()
    {
        ShowAccountsPayable();
        ShowAccountsReceivable();
        ShowTransactions();
    }

    private void ShowAccountsPayable()
    {
        var response = mysqlHelper.GetAccountsPayable();
        if (response == null || response.Count == 0)
            return;

        AccountsPayableEverything.Clear();

        double amountPaid = 0;
        double amountNotPaid = 0;
        double amountPastDue = 0;
        int accountsPastDue = 0;
        double amountTotal = 0;
        //current date time
        DateTime today = DateTime.Now;

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

            amountTotal = amountTotal + amount;

            if (paymentStatus == "Paid")
            {
                amountPaid = amountPaid + amount;
            } else {
                amountNotPaid = amountNotPaid + amount;
            }

            if (today >= dueDate && paymentStatus == "Pending")
            {
                amountPastDue = amountPastDue + amount;
                accountsPastDue = accountsPastDue + 1;
            }




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

           
            ApAmountPaid = amountPaid;
            ApAmountNotPaid = amountNotPaid;
            ApAmountPastDue = amountPastDue;
            ApAmountTotal = amountTotal;
            ApAccountsPastDue = accountsPastDue;
        }
    }


    private void ShowAccountsReceivable()
    {
        var response = mysqlHelper.GetAccountsReceivable();
        if (response == null || response.Count == 0)
            return;

        AccountsReceivableModel.Clear();

    

        double amountPaid = 0;
        double amountNotPaid = 0;
        double amountPastDue = 0;
        int accountsPastDue = 0;
        double amountTotal = 0;
        //current date time
        DateTime today = DateTime.Now;

        foreach (var accountReceivable in response.AsArray())
        {


            string accountsReceivableID = (accountReceivable["AccountsReceivableID"]?.ToString()?.Trim('"'));
            string customerID = (accountReceivable["CustomerID"]?.ToString()?.Trim('"'));
            double amount = double.Parse(accountReceivable["Amount"]?.ToString()?.Trim('"') ?? "0");
            DateTime dueDate = ((DateTime)accountReceivable["DueDate"]);
            string paymentStatus = accountReceivable["PaymentStatus"]?.ToString()?.Trim('"');


            amountTotal = amountTotal + amount;

            if (paymentStatus == "Paid")
            {
                amountPaid = amountPaid + amount;
            }
            else
            {
                amountNotPaid = amountNotPaid + amount;
            }

            if (today >= dueDate && paymentStatus == "Pending")
            {
                amountPastDue = amountPastDue + amount;
                accountsPastDue = accountsPastDue + 1;
            }

            AccountsReceivableModel.Add(new AccountsReceivableModel
            {
                AccountsReceivableID = accountsReceivableID,
                CustomerID = customerID,
                Amount = amount,
                DueDate = dueDate,
                PaymentStatus = paymentStatus
            });

            ArAmountPaid = amountPaid;
            ArAmountNotPaid = amountNotPaid;
            ArAmountPastDue = amountPastDue;
            ArAmountTotal = amountTotal;
            ArAccountsPastDue = accountsPastDue;


        }
    }

    private void ShowTransactions()
    {
        var response = mysqlHelper.GetTransactions();
        if (response == null || response.Count == 0)
            return;

        TransactionEverything.Clear();

        double totalPaid = 0;
        double totalReceived = 0;

        foreach (var transaction in response.AsArray())
        {
            string transactionID = transaction["TransactionID"]?.ToString();
            string accountsPayableID = transaction["AccountsPayableID"]?.ToString();
            string accountsReceivableID = transaction["AccountsReceivableID"]?.ToString();
            double amount = double.Parse(transaction["Amount"]?.ToString() ?? "0");
            DateTime date = ((DateTime)transaction["Date"]);

            if (accountsPayableID != null)
            {
                totalPaid = totalPaid + amount;
            } else if (accountsReceivableID != null)
            {
                totalReceived = totalReceived + amount;
            }


            TransactionEverything.Add(new TransactionEverything
            {
                TransactionID = transactionID,
                AccountsPayableID = accountsPayableID,
                AccountsReceivableID = accountsReceivableID,
                Amount = amount,
                Date = date
            });
        }

        var revenueMinusExpenses = totalReceived - totalPaid;

        TrTotalPaid = totalPaid;
        TrTotalReceived = totalReceived;
        TrRevenueMinusExpenses = revenueMinusExpenses;
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
