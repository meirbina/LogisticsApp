using SMS.Models;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    // NOTE: The placeholder for Fee Transactions remains because a dedicated model for it
    // likely doesn't exist yet. This is a common pattern for aggregated data.
    public class FeeTransactionPlaceholder {
        public string FeesType { get; set; }
        public System.DateTime DueDate { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal Fine { get; set; }
        public decimal Paid { get; set; }
        public decimal Balance => Amount - Paid;
    }

    public class StudentProfileVM
    {
        // The main student object with all its direct relations
        public Student Student { get; set; }

        // Collections for the data in the tabs
        public IEnumerable<FeeTransactionPlaceholder> FeeTransactions { get; set; }
        
        // --- THE FIX: Using your actual models now ---
        public IEnumerable<BookIssue> BookIssues { get; set; }
        public IEnumerable<Exam> Exams { get; set; }
        // --- END FIX ---
    }
}