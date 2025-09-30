namespace SMS.Models.ViewModels;

public class CustomerTypeVM
{
    // For the "Add Customer Type" modal form
    public CustomerType NewCustomerType { get; set; }

    // For the list in the main table
    public IEnumerable<CustomerType> CustomerTypeList { get; set; }
}