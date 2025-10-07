using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class GenericShipment
    {
        public int Id { get; set; }
        public string WaybillNumber { get; set; }
        public string ShipmentType { get; set; }
        public int? MerchantId { get; set; }
        [ForeignKey("MerchantId")]
        public Merchant Merchant { get; set; }
        public string SenderName { get; set; }
        public string SenderPhoneNumber { get; set; }
        public string SenderAddress { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public string ReceiverAddress { get; set; }
        public int DestinationLocationId { get; set; }
        [ForeignKey("DestinationLocationId")]
        public Location DestinationLocation { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalCost { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
        public bool IsCollected { get; set; } = false;
        public DateTime? ReceivedDate { get; set; }
        public string ReceivedBy { get; set; } // Employee Email
        public bool IsCancelled { get; set; } = false;
        public string CancellationReason { get; set; }
        public string CancelledBy { get; set; } // Employee Email
        public DateTime? CancellationDate { get; set; }

        public bool IsModified { get; set; } = false;
        public string ModificationReason { get; set; }
        public string ModifiedBy { get; set; } // Employee Email
        public DateTime? ModificationDate { get; set; }
        public int? ShipmentCollectionId { get; set; }
        [ForeignKey("ShipmentCollectionId")]
        public ShipmentCollection ShipmentCollection {get; set;}
        public virtual ICollection<GenericShipmentItem> Items { get; set; }
    }
}