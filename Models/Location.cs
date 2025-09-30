using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models // Or your appropriate namespace
{
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Location name is required.")]
        [Display(Name = "Location Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a state.")]
        [Display(Name = "State")]
        public int StateId { get; set; }

        [ForeignKey("StateId")]
        public State State { get; set; }

        [Required(ErrorMessage = "Please select a type.")]
        [Display(Name = "Location Type")]
        public string LocationType { get; set; } // "Station" or "Hub"

        [Required(ErrorMessage = "Location code is required.")]
        [Display(Name = "Location Code")]
        public string LocationCode { get; set; }

        [Display(Name = "Address")]
        public string LocationAddress { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        [Display(Name = "Contact Person")]
        public string ContactPersonName { get; set; }

        [Display(Name = "Contact Person Phone")]
        public string ContactPersonPhone { get; set; }

        [Display(Name = "Is Commissioned?")]
        public bool IsCommissioned { get; set; }
    }
}