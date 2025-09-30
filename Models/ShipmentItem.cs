using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class ShipmentItem
{
    public int Id { get; set; }
    public int ShipmentId { get; set; }
    public string Description { get; set; }
    public decimal Weight { get; set; }
    public string Condition { get; set; }
    public int PackagingPriceId { get; set; }
    [ForeignKey("PackagingPriceId")]
    public PackagingPrice PackagingPrice { get; set; }
    public int NumberOfPackagingItems { get; set; }
}