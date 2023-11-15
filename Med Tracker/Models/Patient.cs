using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Med_Tracker.Models
{
    public class Patient
    {
        [Key]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(20)]
        [Index(IsUnique = true)]
        public string NHSNumber { get; set; }

        [Required(ErrorMessage = "Enter a valid email")]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MaxLength(256)]
        public string PasswordHash { get; set; }

        [Compare("PasswordHash", ErrorMessage = "The password and confirmation password do not match.")]
        [Display(Name = "Re-Password")]
        [DataType(DataType.Password)]
        [MaxLength(256)]
        public string Re_PasswordHash { get; set; }

        // Other patient details 
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        public string Confirmed { get; set; }

        public string RegToken { get; set; }

        // Navigation property for medications
        public List<Medication> Medications { get; set; }
    }

}