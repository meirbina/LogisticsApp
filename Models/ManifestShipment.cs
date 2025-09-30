using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    // This is a linking table between Manifest and the two types of Shipments
    public class ManifestShipment
    {
        public int Id { get; set; }
        
        public int ManifestId { get; set; }
        [ForeignKey("ManifestId")]
        public Manifest Manifest { get; set; }

        // We will store the waybill number, which is unique across both shipment types
        public string WaybillNumber { get; set; }
    }
}