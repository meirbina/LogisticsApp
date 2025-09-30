using Microsoft.AspNetCore.Mvc.Rendering;

namespace SMS.Models.ViewModels;

public class DriverVM
{
    // public IEnumerable<SelectListItem> CompanyList { get; set; }
    // public Driver Driver { get; set; }
    
    public IEnumerable<Driver> DriverList { get; set; }
    public Driver NewDriver { get; set; } // For the 'Add New Driver' form
    public IEnumerable<SelectListItem> CompanyList { get; set; }
    
    
    
    
    
}