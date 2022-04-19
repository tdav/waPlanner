using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Расписание")]
    [Route("[controller]")]
    public class SchedulerController : ControllerBase
    {
        private readonly ISchedulerService service;
        public SchedulerController(ISchedulerService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task Insert([FromBody] viScheduler scheduler)
        {
            await service.AddSchedulerAsync(scheduler);
        }

        [HttpPost("{scheduler_id}")]
        public async Task Update([FromBody] viScheduler scheduler)
        {
            await service.UpdateSchedulerAsync(scheduler);
        }

        [HttpPost("{scheduler_id}/{status}/change_status")]
        public async Task UpdateSchedulerStatus(int scheduler_id, int status)
        {
            await service.UpdateSchedulerStatus(scheduler_id, status);
        }

        [HttpGet("{scheduler_id}")]
        public async Task<viScheduler> GetSchedulerById(int scheduler_id)
        {
            return await service.GetSchedulerByIdAsync(scheduler_id);
        }

        [HttpGet("get_by_organization")]
        public async Task<viEvents[]> GetSchedulers()
        {
            return await service.GetAllSchedulersByOrgAsync();
        }
    }
}
