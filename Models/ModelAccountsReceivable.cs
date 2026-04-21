// App:     Amortization calculator
// Element: Model

namespace FISSystem.Models;

public class ModelAccountsReceivable
{
    public int PayNumber { get; set; }
    public decimal PayAmount { get; set; }
    public decimal Principal { get; set; }
    public decimal Interest { get; set; }
    public decimal Balance { get; set; }
}
