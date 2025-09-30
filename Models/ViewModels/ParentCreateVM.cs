using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace SMS.Models.ViewModels
{
    public class ParentCreateVM
    {
        [Required] public string FatherName { get; set; }
        public string? MotherName { get; set; }
        [Required] public string Occupation { get; set; }
        public decimal? Income { get; set; }
        public string? Education { get; set; }
        [Required] public string City { get; set; }
        [Required] public string State { get; set; }
        public string? MobileNo { get; set; }
        public string? Address { get; set; }
        
        [Display(Name = "Profile Picture")]
        public IFormFile? ProfilePicture { get; set; }
        
        [Required, EmailAddress] public string Email { get; set; }
        [Required, DataType(DataType.Password)] public string Password { get; set; }
    }
}