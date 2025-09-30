namespace SMS.Models.ViewModels;

public class PackagingPriceVM
{
    // For the "Add Packaging" modal form
    public PackagingPrice NewPackagingPrice { get; set; }

    // For the list in the main table
    public IEnumerable<PackagingPrice> PackagingPriceList { get; set; }
}