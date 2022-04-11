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
        public async Task Update(int scheduler_id, [FromBody] viScheduler scheduler)
        {
            await service.UpdateSchedulerAsync(scheduler_id, scheduler);
        }

        [HttpPut("change_scheduler_status/{scheduler_id}/{status}")]
        public async Task UpdateSchedulerStatus(int scheduler_id, byte status)
        {
            await service.UpdateSchedulerStatus(scheduler_id, status);
        }

        [HttpGet("{scheduler_id}")]
        public async Task<viScheduler> GetSchedulerById(int scheduler_id)
        {
            return await service.GetSchedulerByIdAsync(scheduler_id);
        }

        [HttpGet("schedulers/{organization_id}")]
        public async Task<viScheduler[]> GetSchedulers(int organization_id)
        {
            return await service.GetAllSchedulersByOrgAsync(organization_id);
        }

        [HttpGet("choose_scheduler_month/{date}/{organization_id}")]
        public async Task<viMiniScheduler[]> ChooseSchedulerMonth(DateTime date, int organization_id)
        {
            return await service.ChooseSchedulerMonth(date, organization_id);
        }

        [HttpGet("choose_scheduler_day/{date}/{organization_id}")]
        public async Task<viMiniScheduler[]> ChooseSchedulerDay(DateTime date, int organization_id)
        {
            return await service.ChooseSchedulerDay(date, organization_id);
        }
    }
}
