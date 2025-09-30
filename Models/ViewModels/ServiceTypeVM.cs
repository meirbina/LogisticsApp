namespace SMS.Models.ViewModels;

public class ServiceTypeVM
{
    // For the "Add Service Type" modal form
    public ServiceType NewServiceType { get; set; }

    // For the list in the main table
    public IEnumerable<ServiceType> ServiceTypeList { get; set; }
}