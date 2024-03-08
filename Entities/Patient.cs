using System.ComponentModel.DataAnnotations;

namespace patient_test_task.Entities
{
    public class Patient: BaseEntity
    {
        public string? Use { get; set; }
        [Required]
        public string Family { get; set; }
        public int? Gender { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        public bool Active { get; set; }
        public List<Given> Givens { get; set; }
    }
}
