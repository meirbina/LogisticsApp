using System.ComponentModel.DataAnnotations;

namespace SMS.Models;

public class Company
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Company Name is required.")]
    public string Name { get; set; }
}