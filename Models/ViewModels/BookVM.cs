using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Models;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class BookVM
    {
        // For the "Create Book" form
        public Book NewBook { get; set; } = new();

        // For the "Books List" table
        public IEnumerable<Book> BookList { get; set; } = new List<Book>();

        // For the <select> dropdowns in the form
        public IEnumerable<SelectListItem>? BranchList { get; set; }
        public IEnumerable<SelectListItem>? CategoryList { get; set; }
        
        // To handle the cover image upload
        public IFormFile? CoverImage { get; set; }
    }
}