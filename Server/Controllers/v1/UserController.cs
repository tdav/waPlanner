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

        [HttpPost]
        public async Task Insert([FromBody] tbUser user)
        {
            await service.InsertAsync(user);
        }

        [HttpPost("add_new_patient")]
        public async Task AddNewPatient(viPatient patient)
        {
            await service.AddPatientsAsync(patient);
        }
       
        [HttpPut]
        public async Task Update([FromBody] tbUser user)
        {
            await service.UpdateAsync(user);
        }

        [HttpPut("change_patient")]
        public async Task UpdatePatient(int patient_id, viPatient patient)
        {
            await service.UpdatePatient(patient_id, patient);
        }

        [HttpPut("change_patient_status")]
        public async Task UpdatePatientStatus(int patient_id, byte status)
        {
            await service.UpdatePatientStatus(patient_id, status);
        }

        [HttpDelete("id")]
        public void Delete(int id)
        {
            service.Delete(id);
        }

        [HttpGet("id")]
        public async Task<tbUser> GetUserById(int id)
        {
            return await service.GetUserByIdAsync(id);
        }

        [HttpGet("get_patients")]
        public async Task<List<viPatient>> GetPateintsAsync(int organization_id)
        {
            return await service.GetPateintsAsync(organization_id);
        }

        [HttpGet]
        public async Task<tbUser[]> GetAll()
        {
            return await service.GetAllAsync();
        }
    }
}
