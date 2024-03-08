using patient_test_task.Entities;
using patient_test_task.Interfaces;

namespace patient_test_task.Database
{
    public class PatientRepository : IPatientRepository
    {
        protected readonly ApplicationContext _context;
        public PatientRepository(ApplicationContext context) => _context = context;
        public Guid UpdatePatient(Patient patient, List<string> givens)
        {
           _context.Patients.Attach(patient);
            patient.Givens.Clear();
            var existedGiven = _context.Givens.Where(g => givens.Contains(g.Record));
            var givenDicitionary = existedGiven.ToDictionary(g => g.Record, g => g.Id);
            var newGivens = givens.Except(givenDicitionary.Keys).Select(g => new Given() { Record = g});
            patient.Givens.AddRange(existedGiven);
            patient.Givens.AddRange(newGivens);
            _context.Update(patient);
            _context.SaveChanges();
            return patient.Id;

        }
    }
}
