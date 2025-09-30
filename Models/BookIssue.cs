using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public enum BookStatus
    {
        Issued,
        Returned
    }

    public class BookIssue
    {
        public int Id { get; set; }

        [Required]
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }

        [Required]
        public int BookId { get; set; }
        public Book? Book { get; set; }

        [Required]
        public string ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        
        // We store the role at the time of issue for historical reports
        public string Role { get; set; }

        [Required]
        public DateTime IssueDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }
        
        public DateTime? ReturnDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FineAmount { get; set; } = 0;

        [Required]
        public BookStatus Status { get; set; }
    }
}