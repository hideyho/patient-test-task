using LinqKit;
using patient_test_task.DTO;
using patient_test_task.Entities;
using patient_test_task.Enums;
using patient_test_task.Interfaces;
using patient_test_task.Models;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace patient_test_task.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<Patient> _patientRepository;
        private readonly IRepository<Given> _givenRepository;
        private readonly IPatientRepository _additionalPatientRepository;
        public PatientService(IRepository<Patient> patientRepository, IRepository<Given> givenRepository, IPatientRepository additionalPatientRepository)
        {
            _patientRepository = patientRepository;
            _givenRepository = givenRepository;
            _additionalPatientRepository = additionalPatientRepository;
        }

        public bool CreateMany(List<PatientModel> patients)
        {
            try
            {
                var recievedGiven = new List<string>();
                var genderDictionary = Enum.GetValues(typeof(GenderEnum)).Cast<GenderEnum>().ToDictionary(d => d.ToString(), d => (int)d);
                patients.ForEach(p => recievedGiven.AddRange(p.Name.Given));
                recievedGiven = recievedGiven.Distinct().ToList();
                var existedGiven = _givenRepository.GetListResultSpec(g => g.Where(g => recievedGiven.Contains(g.Record))).ToList();
                var existedGivenDictionary = existedGiven.ToDictionary(g => g.Record, g => g.Id);
                var notExistedGiven = recievedGiven.Except(existedGivenDictionary.Keys).Select(g => new Given() { Record = g}).ToList();

                var given = _givenRepository.CreateMany(notExistedGiven).ToList();
                given.AddRange(existedGiven);

                var newPatients = patients.Select(p => new Patient()
                {
                    Active = p.Active,
                    BirthDate = p.BirthDate,
                    Family = p.Name.Family,
                    Gender = String.IsNullOrWhiteSpace(p.Gender) ? (int)GenderEnum.Unknown: genderDictionary[p.Gender],
                    Use = p.Name.Use,
                    Givens = given.Where(g => p.Name.Given.Contains(g.Record)).ToList()
                });
                 _patientRepository.CreateMany(newPatients);
                return true;

            }
            catch(Exception ex)
            {
                return false;
            }
        }


        public Guid CreatePatient(PatientModel model)
        {
            GenderEnum gender;
            if (model.Gender != null)
            {
                if (!Enum.TryParse(model.Gender, true, out gender))
                {
                    return Guid.Empty;
                }
            }
            else
                gender = GenderEnum.Unknown;

            var existedGiven = _givenRepository.GetListResultSpec(g => g.Where(s => model.Name.Given.Contains(s.Record)));
            var existedGivenDictionary = existedGiven.ToDictionary(g => g.Record, g => g.Id);
            var notExistedGiven = model.Name.Given.Except(existedGivenDictionary.Keys).Select(g => new Given() { Record = g}).ToList();
            var given = _givenRepository.CreateMany(notExistedGiven).ToList();
            given.AddRange(existedGiven);

            var patient = new Patient()
            {
                Active = model.Active,
                BirthDate = model.BirthDate,
                Family = model.Name.Family,
                Gender = (int)gender,
                Givens = given,
                Use = model.Name.Use
            };

            return _patientRepository.Create(patient).Id;
        }

        public Guid DeletePatient(Guid id)
        {
            var patient = _patientRepository.GetById(id);
            
            if (patient == null)
                return Guid.Empty;

            _patientRepository.Delete(patient);
            return id;
        }

        public List<PatientModel> GetAllPatients()
        {
            throw new NotImplementedException();
        }

        public PatientModel GetPatientById(Guid id)
        {
            var patient = _patientRepository.GetById(id);
            var given = _givenRepository.GetListResultSpec(g => g.Where(g => g.Patients.Contains(patient)).Select(g => g.Record)).ToList();

            if (patient == null)
                return null;

            return new PatientModel()
            {
                Active = patient.Active,
                BirthDate = patient.BirthDate,
                Gender = ((GenderEnum)patient.Gender).ToString(),
                Name = new NameModel()
                {
                    Family = patient.Family,
                    Use = patient.Use,
                    Given = given,
                    Id = patient.Id
                }
            };
        }

        public List<PatientModel> GetPatientsWithFilter(List<KeyValuePair<string, DateModel>> filters)
        {
            var predicate = CreatePredicate(filters);
            var patients = _patientRepository.GetListResultSpec(p => p.Where(predicate).Select(p => new PatientModel()
            {
                Name = new NameModel()
                {
                    Id = p.Id,
                    Family = p.Family,
                    Given = p.Givens.Select(g => g.Record).ToList(),
                    Use = p.Use
                },
                BirthDate = p.BirthDate,
                Active = p.Active,
                Gender = ((GenderEnum)p.Gender).ToString(),
            })).ToList();
            

            return patients;
        }

        public Guid UpdatePatient(PatientModel model)
        {
            GenderEnum gender;
            if (model.Gender != null)
            {
                if (!Enum.TryParse(model.Gender, true, out gender))
                {
                    return Guid.Empty;
                }
            }
            else
                gender = GenderEnum.Unknown;

            var patient = _patientRepository.GetResultSpec(p => p.Where(p => p.Id.Equals(model.Name.Id)).Select(p => new Patient()
            {
                Id = p.Id,
                Active = model.Active,
                BirthDate = model.BirthDate,
                Gender = (int)gender,
                Family = model.Name.Family,
                Givens = p.Givens,
                Use = model.Name.Use
            }).SingleOrDefault());

            return _additionalPatientRepository.UpdatePatient(patient, model.Name.Given);
        }

        private Expression<Func<Patient, bool>> CreatePredicate(List<KeyValuePair<string, DateModel>> filters)
        {
            var predicate = PredicateBuilder.True<Patient>();

            foreach (var filter in filters)
            {
                switch (filter.Key)
                {
                    case "eq":
                        predicate = filter.Value.Time != null
                        ? predicate.And(patient => patient.BirthDate.Equals(filter.Value.Date.ToDateTime((TimeOnly)filter.Value.Time)))
                        : predicate.And(patient => DateOnly.FromDateTime(patient.BirthDate).Equals(filter.Value.Date));
                        break;
                    case "ne":
                        predicate = filter.Value.Time != null
                        ? predicate.And(patient => !patient.BirthDate.Equals(filter.Value.Date.ToDateTime((TimeOnly)filter.Value.Time)))
                        : predicate.And(patient => !DateOnly.FromDateTime(patient.BirthDate).Equals(filter.Value.Date));
                        break;
                    case "gt":
                        predicate = predicate.And(patient => patient.BirthDate > filter.Value.Date.ToDateTime(filter.Value.Time != null ? (TimeOnly)filter.Value.Time : TimeOnly.MinValue));
                        break;
                    case "lt":
                        predicate = predicate.And(patient => patient.BirthDate < filter.Value.Date.ToDateTime(filter.Value.Time != null ? (TimeOnly)filter.Value.Time : TimeOnly.MaxValue));
                        break;
                    case "ge":
                        predicate = predicate.And(patient => patient.BirthDate >= filter.Value.Date.ToDateTime(filter.Value.Time != null ? (TimeOnly)filter.Value.Time : TimeOnly.MinValue));
                        break;
                    case "le":
                        predicate = predicate.And(patient => patient.BirthDate <= filter.Value.Date.ToDateTime(filter.Value.Time != null ? (TimeOnly)filter.Value.Time : TimeOnly.MaxValue));
                        break;
                    case "sa":
                        predicate = predicate.And(patient => DateOnly.FromDateTime(patient.BirthDate.Date) > filter.Value.Date);
                        break;
                    case "eb":
                        predicate = predicate.And(patient => DateOnly.FromDateTime(patient.BirthDate.Date) < filter.Value.Date);
                        break;
                    case "ap":
                        predicate = predicate.And(patient => DateOnly.FromDateTime(patient.BirthDate.Date).Equals(filter.Value.Date));
                        break;
                    default: break;
                }
            }

            return predicate;
        }
    }
}
