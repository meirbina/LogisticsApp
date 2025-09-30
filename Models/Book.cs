using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch? Branch { get; set; }
        
        [Required(ErrorMessage = "Book title is required.")]
        [Display(Name = "Book Title")]
        public string Title { get; set; }
        
        [Display(Name = "ISBN No")]
        public string? Isbn { get; set; }

        public string? Author { get; set; }
        
        public string? Edition { get; set; }
        
        [Required(ErrorMessage = "Purchase date is required.")]
        [Display(Name = "Purchase Date")]
        public DateTime PurchaseDate { get; set; }
        
        [Required(ErrorMessage = "Please select a book category.")]
        [Display(Name = "Book Category")]
        public int BookCategoryId { get; set; }
        public BookCategory? BookCategory { get; set; }
        
        [Required(ErrorMessage = "Publisher is required.")]
        public string Publisher { get; set; }
        
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Price is required.")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        
        [Display(Name = "Cover Image")]
        public string? CoverImageUrl { get; set; }
        
        [Required(ErrorMessage = "Total stock is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative.")]
        [Display(Name = "Total Stock")]
        public int TotalStock { get; set; }
    }
}