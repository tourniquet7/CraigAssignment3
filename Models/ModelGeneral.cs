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

    public class TransactionEverything : Transaction
    {
        public string AccountsReceivableID { get; set; }
        public string AccountsPayableID { get; set; }
    }


    public class AccountsPayableModel
    {
        public string AccountsPayableID { get; set; }
        public double Amount { get; set; }
        public string DueDate { get; set; }
        public string PaymentStatus { get; set; }
        public bool ButtonIsVisible { get; set; }
        public Color PastDueColor { get; set; }

    }

    public class AccountsPayableEverything : AccountsPayableModel
    {
        public string RawMaterialID { get; set; }
        public int RawMaterialQty { get; set; }
        public string VendorID { get; set; }
        public string EmployeeId { get; set; }
        public bool EmployeeDirectDeposit { get; set; }
    }

   
 
}
