using System;
using System.ComponentModel.DataAnnotations;

namespace Med_Tracker.Models
{
    public class Medication
    {
        [Key]
        public int MedicationId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [MaxLength(255)]
        public string MedicationName { get; set; }

        [Required]
        public string MedicationType { get; set; }

        [Required]
        [MaxLength(50)]
        public string Dosage { get; set; }

        [Required]
        [MaxLength(50)]
        public string Frequency { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

    }

}