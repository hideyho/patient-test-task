using patient_test_task.Entities;

namespace patient_test_task.Interfaces
{
    public interface IPatientRepository
    {
        Guid UpdatePatient(Patient patient, List<string> givens);
    }
}
