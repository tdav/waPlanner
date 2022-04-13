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

        [HttpPost("add_new_user")]
        public async Task AddNewPatient(viPatient patient)
        {
            await service.AddPatientsAsync(patient);
        }

        [HttpPost("change_user")]
        public async Task UpdatePatient(viPatient patient)
        {
            await service.UpdatePatient(patient);
        }

        [HttpPost("change_user_status/{status}")]
        public async Task UpdatePatientStatus(viPatient patient, byte status)
        {
            await service.UpdatePatientStatus(patient, status);
        }

        [HttpGet("get_users/{organization_id}")]
        public async Task<List<viPatient>> GetPateintsAsync(int organization_id)
        {
            return await service.GetAllPateintsAsync(organization_id);
        }

        [HttpGet("{user_id}")]
        public async Task<viPatient> GetAll(int user_id)
        {
            return await service.GetPatientAsync(user_id);
        }

        [HttpGet("get_all")]
        public async Task<viPatient[]> GetUsers()
        {
            return await service.GetUsers();
        }
    }
}
