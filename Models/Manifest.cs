using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Manifest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ManifestId { get; set; }

        public int DepartureLocationId { get; set; }
        [ForeignKey("DepartureLocationId")]
        public Location DepartureLocation { get; set; }

        public int DestinationLocationId { get; set; }
        [ForeignKey("DestinationLocationId")]
        public Location DestinationLocation { get; set; }

        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }

        // --- NEW & UPDATED SIGN-OFF FIELDS ---
        [Column(TypeName = "decimal(18,2)")]
        public decimal DispatchFee { get; set; }

        public int? DriverId { get; set; }
        [ForeignKey("DriverId")]
        public Employee Driver { get; set; } // Link to the Employee table

        public int? VehicleId { get; set; }
        [ForeignKey("VehicleId")]
        public Vehicle Vehicle { get; set; }
        
        public DateTime? DispatchedDate { get; set; }
        public string DispatchedBy { get; set; }
        public bool IsSignedOff { get; set; } = false;

        public virtual ICollection<ManifestShipment> ManifestShipments { get; set; }
    }
}