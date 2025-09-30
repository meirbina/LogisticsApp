using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SMS.Models
{
    public class Student
    {
        public int Id { get; set; }

        // Academic Details
        [Required]
        public string AcademicYear { get; set; }
        [Required]
        public string RegisterNo { get; set; }
        [Required]
        public string Roll { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime AdmissionDate { get; set; }
        [Required]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        [Required]
        public int ControlClassId { get; set; }
        public ControlClass ControlClass { get; set; }
        [Required]
        public int SectionId { get; set; }
        public Section Section { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // Student Details
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        public string BloodGroup { get; set; }
        public string Religion { get; set; }
        public string MotherTongue { get; set; }
        public string Caste { get; set; }
        [Required]
        public string MobileNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PresentAddress { get; set; }
        public string PermanentAddress { get; set; }
        public string ProfilePictureUrl { get; set; }
        
        // --- FIX: Added the missing Previous School properties ---
        public string? PreviousSchoolName { get; set; }
        public string? PreviousSchoolQualification { get; set; }
        public string? PreviousSchoolRemarks { get; set; }
        // --- END FIX ---

        // Foreign key to Parent table
        [Required]
        public int ParentId { get; set; }
        public Parent Parent { get; set; }
        
        // Foreign key to ApplicationUser table for login
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }

        // Optional Foreign Keys
        public int? RouteMasterId { get; set; }
        public RouteMaster RouteMaster { get; set; }
        public int? VehicleMasterId { get; set; }
        public VehicleMaster VehicleMaster { get; set; }
        public int? HostelMasterId { get; set; }
        public HostelMaster HostelMaster { get; set; }
        public int? HostelRoomId { get; set; }
        public HostelRoom HostelRoom { get; set; }
    }
}