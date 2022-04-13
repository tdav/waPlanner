using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
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
    [SwaggerTag("Расписание")]
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

        [HttpPost("change_scheduler_status/{status}")]
        public async Task UpdateSchedulerStatus([FromBody] viScheduler scheduler, byte status)
        {
            await service.UpdateSchedulerStatus(scheduler, status);
        }

        [HttpGet("{scheduler_id}")]
        public async Task<viScheduler> GetSchedulerById(int scheduler_id)
        {
            return await service.GetSchedulerByIdAsync(scheduler_id);
        }

        [HttpGet("get_by_organization/{organization_id}")]
        public async Task<viEvents[]> GetSchedulers(int organization_id)
        {
            return await service.GetAllSchedulersByOrgAsync(organization_id);
        }
    }
}
