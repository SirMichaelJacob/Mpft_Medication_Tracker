using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Med_Tracker.Models
{
    [NotMapped]
    public class AdminViewModel
    {

        public List<Patient> Patients { get; set; }

        public List<Medication> Medications { get; set; }
    }
}