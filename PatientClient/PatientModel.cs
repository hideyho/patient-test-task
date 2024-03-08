using System.ComponentModel.DataAnnotations;

namespace PatientClient
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