using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models;

public class Shipment
{
    public int Id { get; set; }
    public string WaybillNumber { get; set; }
    public string SenderName { get; set; }
    public string SenderPhoneNumber { get; set; }
    public string SenderAddress { get; set; }
    public string ReceiverName { get; set; }
    public string ReceiverPhoneNumber { get; set; }
    public string ReceiverAddress { get; set; }
    public int ReceiverStateId { get; set; }
    public int DestinationLocationId { get; set; }
    [ForeignKey("DestinationLocationId")]
    public Location DestinationLocation { get; set; }
    public string PaymentMethod { get; set; }
    public decimal DeclaredValue { get; set; }
    public int? InsuranceId { get; set; }
    public decimal ShipmentCost { get; set; }
    public decimal PackagingCost { get; set; }
    public decimal InsuranceCost { get; set; }
    public decimal Vat { get; set; }
    public decimal TotalCost { get; set; }
    public string Status { get; set; }
    public DateTime DateCreated { get; set; }
    public string CreatedBy { get; set; }
    // --- NEW PROPERTIES FOR RECEIVING AND COLLECTION ---
    public DateTime? ReceivedDate { get; set; }
    public string ReceivedBy { get; set; } // Employee Email

    public bool IsCollected { get; set; } = false;

    // We will link to a separate collection details record
    public int? ShipmentCollectionId { get; set; }
    [ForeignKey("ShipmentCollectionId")]
    public ShipmentCollection ShipmentCollection { get; set; }
    public virtual ICollection<ShipmentItem> Items { get; set; }
}