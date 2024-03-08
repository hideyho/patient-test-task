using patient_test_task.DTO;
using patient_test_task.Models;

namespace patient_test_task.Interfaces
{
    public interface IPatientService
    {
        Guid CreatePatient(PatientModel model);
        Guid UpdatePatient(PatientModel model);
        List<PatientModel> GetAllPatients();
        List<PatientModel> GetPatientsWithFilter(List<KeyValuePair<string, DateModel>> filters);
        Guid DeletePatient(Guid id);
        PatientModel GetPatientById(Guid id);
        bool CreateMany(List<PatientModel> patients);
    }
}
