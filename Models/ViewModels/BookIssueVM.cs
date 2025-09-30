using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class BookIssueVM
    {
        // For the "Book Issue" form
        public BookIssue NewBookIssue { get; set; } = new();

        // For the "Books List" table
        public IEnumerable<BookIssue> IssuedBooksList { get; set; } = new List<BookIssue>();

        // For the <select> dropdowns in the form
        public IEnumerable<SelectListItem>? BranchList { get; set; }
        public IEnumerable<SelectListItem>? RoleList { get; set; }

        // --- THIS IS THE CRITICAL ADDITION ---
        // This property will capture the value from the "Book Category" dropdown,
        // which is necessary for the cascading logic to work and for model validation.
        [Required(ErrorMessage = "Please select a book category.")]
        [Display(Name = "Book Category")]
        public int BookCategoryId { get; set; }
    }
}