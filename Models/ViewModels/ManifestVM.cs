using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class ManifestVM
    {
        // For the filter form
        [Required]
        [Display(Name = "Departure Location")]
        public int DepartureLocationId { get; set; }
        [Required]
        [Display(Name = "Destination Location")]
        public int DestinationLocationId { get; set; }
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        // For displaying the generated Manifest ID
        public string GeneratedManifestId { get; set; }
        public int ManifestDbId { get; set; } // The primary key of the manifest record

        // List of available shipments that match the filter
        public List<ShipmentDisplayViewModel> AvailableShipments { get; set; }
        // List of shipments already added to the current manifest
        public List<ShipmentDisplayViewModel> ManifestedShipments { get; set; }

        // For populating dropdowns
        public IEnumerable<SelectListItem> LocationList { get; set; }
        
        // For the sign-off modal
        public IEnumerable<SelectListItem> VehicleList { get; set; }
    }
}