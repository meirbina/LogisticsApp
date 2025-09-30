namespace SMS.Models.ViewModels;

public class CompanyVM
{
    public Company NewCompany { get; set; } = new Company();

    // For the "Company List" on the right
    public IEnumerable<Company> CompanyList { get; set; } = new List<Company>();
}