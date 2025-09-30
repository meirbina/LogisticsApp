using System;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    // Represents a single, unified row in the transaction list.
    public class TransactionRow
    {
        public string BranchName { get; set; }
        public string AccountName { get; set; }
        public string Type { get; set; } // "Deposit" or "Expense"
        public string VoucherHeadName { get; set; }
        public string RefNo { get; set; }
        public string Description { get; set; }
        public string PayVia { get; set; }
        public decimal Amount { get; set; } // The base transaction amount
        public decimal Dr { get; set; } // Debit (Expense)
        public decimal Cr { get; set; } // Credit (Deposit)
        public decimal Balance { get; set; } // The running balance after this transaction
        public DateTime Date { get; set; }
    }

    // The main ViewModel for the entire page.
    public class TransactionVM
    {
        public List<TransactionRow> Transactions { get; set; } = new List<TransactionRow>();
        public decimal FinalBalance { get; set; }
    }
}