using System.Data.Entity;

namespace Med_Tracker.Models
{
    public class MyDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public DbSet<Medication> Medications { get; set; }

        public DbSet<Provider> Providers { get; set; }
    }
}