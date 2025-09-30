using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Parent
    {
        public int Id { get; set; }

        [Required]
        public string FatherName { get; set; }
        public string? MotherName { get; set; }
        [Required]
        public string Occupation { get; set; }
        public decimal? Income { get; set; }
        public string? Education { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        public string? MobileNo { get; set; } 
        public string? Address { get; set; }
        public string? ProfilePictureUrl { get; set; }

        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}