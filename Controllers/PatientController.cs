using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using patient_test_task.DTO;
using patient_test_task.Interfaces;
using patient_test_task.Models;

namespace patient_test_task.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }


        /// <summary>
        /// Create patient
        /// </summary>
        /// <param name="model">Patients</param>
        /// <returns></returns>
        /// <response code="400"> Model is not valid </response>
        /// <response code="404"> Gender not found </response>
        [HttpPost("create")]
        public IActionResult Create(PatientModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var patientId = _patientService.CreatePatient(model);
            if(patientId.Equals(Guid.Empty))
            {
                return NotFound("Gender is not exist");
            }

            return Ok(patientId);
        }

        /// <summary>
        /// Search patients by BirthDate with H7 rules
        /// </summary>
        /// <param name="searchString">BirthDate search string</param>
        /// <remarks>Rules: https://www.hl7.org/fhir/search.html#date
        ///     
        ///     GET /search/gt2022-03-08T13:34:45&amp;lt2024-03-08T13:34:45 
        ///     
        ///     
        /// </remarks>
        /// <returns></returns>
        /// <response code="400"> Model is not valid </response>
        [HttpGet("search")]
        public IActionResult Search(string? searchString)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var filters = new List<KeyValuePair<string, DateModel>>();

            if(!String.IsNullOrWhiteSpace(searchString))
            {
                var options = searchString.Split('&');
                foreach (var option in options)
                {
                    var tag = option.Substring(0, 2);
                    var date = option.Substring(2).Split('T');
                    var dateModel = new DateModel()
                    {
                        Date = DateOnly.Parse(date[0]),
                        Time = date.Length > 1 ? TimeOnly.Parse(date[1]) : null
                    };
                    filters.Add(new KeyValuePair<string, DateModel>(tag, dateModel));
                }
            } 

            var result = _patientService.GetPatientsWithFilter(filters);
            return Ok(result);
        }

        /// <summary>
        /// Create many patients
        /// </summary>
        /// <param name="models">Patients</param>
        /// <returns></returns>
        /// <response code="400">Model is not valid or empty</response>
        /// <response code="500">Server side error</response>
        [HttpPost("createRange")]
        public IActionResult CreatePatientsMany(List<PatientModel> models)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (models == null || models.Count == 0)
                return BadRequest("Nothing to add");

            if (_patientService.CreateMany(models))
                return Ok();

            else return StatusCode(500);
        }

        /// <summary>
        /// Delete patient
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="400">Model is not valid</response>
        /// <response code="404">Patient not found</response>
        [HttpDelete("delete")]
        public IActionResult DeletePatient(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = _patientService.DeletePatient(id);
            if (result == Guid.Empty)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Get patient by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="400">Model is not valid</response>
        /// <response code="404">Patient not found</response>
        [HttpGet("getById")]
        public IActionResult GetPatient(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = _patientService.GetPatientById(id);
            
            if(result == null)
                return NotFound();
            
            return Ok(result);
        }

        /// <summary>
        /// Update patient
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <response code="400">Model is not valid</response>
        /// <response code="404">Patient not found</response>
        [HttpPut("update")]
        public IActionResult UpdatePatient(PatientModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var result = _patientService.UpdatePatient(model);
            if (result == Guid.Empty)
                return NotFound();
            
            return Ok(result);
        }

    }
}
