using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Пользователи")]
    [Route("api/[controller]")]
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
            await service.AddAsync(patient);
        }

        [HttpPost("change_user")]
        public async Task UpdatePatient(viPatient patient)
        {
            await service.UpdateAsync(patient);
        }

        [HttpPost("change_user_status/{status}")]
        public async Task UpdatePatientStatus(viPatient patient, byte status)
        {
            await service.SetStatusAsync(patient, status);
        }

        [HttpGet("get_users/{organization_id}")]
        public async Task<List<viPatient>> GetPateintsAsync(int organization_id)
        {
            return await service.GetAllAsync(organization_id);
        }

        [HttpGet("{user_id}")]
        public async Task<viPatient> GetAll(int user_id)
        {
            return await service.GetAsync(user_id);
        }
    }
}
