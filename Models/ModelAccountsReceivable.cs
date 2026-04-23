// App:     Amortization calculator
// Element: Model

namespace FISSystem.Models;

public class AccountsReceivableModel
{
    public string AccountsReceivableID { get; set; }
    public string CustomerID { get; set; }
    public double Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string PaymentStatus { get; set; }

    public bool ButtonIsVisible { get; set; }
    public Color PastDueColor { get; set; }
}


public class TransactionReceivable : Transaction
{
    public string AccountsReceivableID{ get; set; }
}
