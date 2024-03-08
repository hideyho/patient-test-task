using System.ComponentModel.DataAnnotations;

namespace PatientClient
{
    public class NameModel
    {
        public Guid Id { get; set; }
        public string? Use { get; set; }
        [Required]
        public string Family { get; set; }
        public List<string>? Given { get; set; }
    }
}
