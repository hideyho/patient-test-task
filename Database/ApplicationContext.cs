using Microsoft.EntityFrameworkCore;
using patient_test_task.Entities;

namespace patient_test_task.Database
{
    public class ApplicationContext:DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { 
            Database.Migrate();
        }
        public DbSet<Given> Givens { get; set; }
        public DbSet<Patient> Patients { get; set; }
    }
}
