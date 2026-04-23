using System;
using System.Collections.Generic;
using System.Text;

namespace FISSystem.Models
{
    public class Transaction
    {
        public string TransactionID { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
