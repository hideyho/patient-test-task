using System.ComponentModel.DataAnnotations;

namespace patient_test_task.Entities
{
    public class Given:BaseEntity
    {
        [Required]
        public string Record { get; set; }
        public List<Patient> Patients { get; set; }
    }
}
