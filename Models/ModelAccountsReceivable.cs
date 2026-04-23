// App:     Amortization calculator
// Element: Model

namespace FISSystem.Models;

public class AccountsReceivable
{
    public string AccountsReceivableID { get; set; }
    public string CustomerID { get; set; }
    public double Amount { get; set; }
    public DateTime DueDate { get; set; }
    public string PaymentStatus { get; set; }
}