using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class GenericShipmentItem
    {
        public int Id { get; set; }
        public int GenericShipmentId { get; set; }
        [ForeignKey("GenericShipmentId")]
        public GenericShipment GenericShipment { get; set; }

        public int GenericPackageId { get; set; }
        [ForeignKey("GenericPackageId")]
        public GenericPackage GenericPackage { get; set; }

        public int Quantity { get; set; }
    }
}