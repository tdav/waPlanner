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
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpPost("new")]
        public async Task<viPatient> AddNewPatient(viPatient patient)
        {
           return await service.AddAsync(patient);
        }

        [HttpPost("change")]
        public async Task UpdatePatient(viPatient patient)
        {
            await service.UpdateAsync(patient);
        }

        [HttpPost("{status}/change")]
        public async Task UpdatePatientStatus(viPatient patient, byte status)
        {
            await service.SetStatusAsync(patient, status);
        }

        [HttpGet("all")]
        public async Task<viPatient[]> GetPateintsAsync()
        {
            return await service.GetAllAsync();
        }

        [HttpGet("{user_id}")]
        public async Task<viPatient> GetAll(int user_id)
        {
            return await service.GetAsync(user_id);
        }
    }
}
