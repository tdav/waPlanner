using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Пользователи")]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpPost("add_new_patient")]
        public async Task AddNewPatient(viPatient patient)
        {
            await service.AddPatientsAsync(patient);
        }

        [HttpPost("change_patient/{patient_id}")]
        public async Task UpdatePatient(int patient_id, viPatient patient)
        {
            await service.UpdatePatient(patient_id, patient);
        }

        [HttpPost("change_patient_status{patient_id}/{status}")]
        public async Task UpdatePatientStatus(int patient_id, byte status)
        {
            await service.UpdatePatientStatus(patient_id, status);
        }

        [HttpGet("get_patients/{organization_id}")]
        public async Task<List<viPatient>> GetPateintsAsync(int organization_id)
        {
            return await service.GetAllPateintsAsync(organization_id);
        }

        [HttpGet("{patient_id}")]
        public async Task<viPatient> GetAll(int patient_id)
        {
            return await service.GetPatientAsync(patient_id);
        }
    }
}
