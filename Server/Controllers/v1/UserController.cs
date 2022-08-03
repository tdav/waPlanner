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

        [HttpPost]
        public async Task<Answer<viPatient>> AddNewPatient([FromBody] viPatient patient)
        {
            return await service.AddAsync(patient);
        }

        [HttpPost("change")]
        public async Task<AnswerBasic> UpdatePatient([FromBody] viPatient patient)
        {
            return await service.UpdateAsync(patient);
        }

        [HttpPost("change_status")]
        public async Task<AnswerBasic> UpdatePatientStatus(viSetStatus status)
        {
            return await service.SetStatusAsync(status);
        }

        [HttpGet]
        public async Task<Answer<List<viPatient>>> GetAll()
        {
            return await service.GetAllAsync();
        }

        [HttpGet("{user_id}")]
        public async Task<Answer<viPatient>> GetPateintsAsync(int user_id)
        {
            return await service.GetAsync(user_id);
        }

        [HttpGet("search/{name}")]
        public async Task<Answer<List<viPatient>>> SearchAsync(string name)
        {
            return await service.SearchUserAsync(name);
        }
    }
}
