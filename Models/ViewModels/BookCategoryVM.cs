using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Models;
using System.Collections.Generic;

namespace SMS.Models.ViewModels
{
    public class BookCategoryVM
    {
        public IEnumerable<BookCategory> CategoryList { get; set; } = new List<BookCategory>();
        public BookCategory NewCategory { get; set; } = new();
        public IEnumerable<SelectListItem>? BranchList { get; set; }
    }
}