using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class ExamHall
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Hall No is required.")]
        [Display(Name = "Hall No")]
        public string HallNo { get; set; }

        [Required(ErrorMessage = "Number of seats is required.")]
        [Display(Name = "No Of Seats")]
        public int NoOfSeats { get; set; }

        [Required(ErrorMessage = "Please select a branch.")]
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}