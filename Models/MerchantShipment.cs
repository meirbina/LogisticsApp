using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class MerchantShipment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string WaybillNumber { get; set; }

        // Foreign key to the Merchant (who is the sender)
        [Required]
        public int MerchantId { get; set; }
        [ForeignKey("MerchantId")]
        public Merchant Merchant { get; set; }

        // Receiver Info (as before)
        [Required] public string ReceiverName { get; set; }
        [Required] public string ReceiverPhoneNumber { get; set; }
        [Required] public string ReceiverAddress { get; set; }
        [Required] public int ReceiverStateId { get; set; }
        [Required] public int DestinationLocationId { get; set; }
        [ForeignKey("DestinationLocationId")]
        public Location DestinationLocation { get; set; }

        // Financial Info (as before)
        public string PaymentMethod { get; set; }
        public decimal DeclaredValue { get; set; }
        public int? InsuranceId { get; set; }
        public decimal ShipmentCost { get; set; }
        public decimal PackagingCost { get; set; }
        public decimal InsuranceCost { get; set; }
        public decimal Vat { get; set; }
        public decimal TotalCost { get; set; }
        
        // Tracking Info
        public string Status { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; } // The logged-in employee's email
        // --- NEW PROPERTIES FOR RECEIVING AND COLLECTION ---
        public DateTime? ReceivedDate { get; set; }
        public string ReceivedBy { get; set; } // Employee Email

        public bool IsCollected { get; set; } = false;

        // We will link to a separate collection details record
        public int? ShipmentCollectionId { get; set; }
        [ForeignKey("ShipmentCollectionId")]
        public ShipmentCollection ShipmentCollection { get; set; }

        public virtual ICollection<MerchantShipmentItem> Items { get; set; }
    }
}