using patient_test_task.Models;
using System.ComponentModel.DataAnnotations;

namespace patient_test_task.DTO
{
    public class PatientModel
    {
        [Required]
        public NameModel Name { get; set; }
        public string? Gender { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }


    }
}