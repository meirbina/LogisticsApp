using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Expense
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }

        [Required]
        [Display(Name = "Account")]
        public int AccountId { get; set; }
        public Accounting Account { get; set; }

        [Required]
        [Display(Name = "Voucher Head")]
        public int VoucherHeadId { get; set; }
        public VoucherHead VoucherHead { get; set; }

        public string Ref { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required]
        public string PayVia { get; set; }

        public string Description { get; set; }

        public string AttachmentUrl { get; set; }
    }
}