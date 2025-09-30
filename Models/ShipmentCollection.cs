using System;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    // A separate table to store details of who collected a shipment.
    public class ShipmentCollection
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string WaybillNumber { get; set; }

        [Required]
        public string CollectedByName { get; set; }
        [Required]
        public string CollectedByPhone { get; set; }
        public string CollectedByAddress { get; set; }
        public string MeansOfId { get; set; }
        public string Comment { get; set; }

        public DateTime CollectionDate { get; set; }
        public string ReleasedBy { get; set; } // Employee Email
    }
}