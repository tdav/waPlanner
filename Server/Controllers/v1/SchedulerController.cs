using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
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
        public async Task<Answer<int>> Insert([FromBody] viScheduler scheduler)
        {
            return await service.AddSchedulerAsync(scheduler);
        }

        [HttpPost("{scheduler_id}")]
        public async Task<Answer<viScheduler>> Update([FromBody] viScheduler scheduler)
        {
            return await service.UpdateSchedulerAsync(scheduler);
        }

        [HttpPost("change_status/{scheduler_id}/{status}")]
        public async Task<AnswerBasic> UpdateSchedulerStatus(int scheduler_id, int status)
        {
            return await service.UpdateSchedulerStatus(scheduler_id, status);
        }

        [HttpGet("{user_id}")]
        public async Task<Answer<viScheduler[]>> GetSchedulerByUserId(int user_id)
        {
            return await service.GetSchedulerByUserIdAsync(user_id);
        }

        [HttpGet("get_by_organization")]
        public async Task<Answer<viEvents[]>> GetSchedulers()
        {
            return await service.GetAllSchedulersByOrgAsync();
        }

        [HttpGet("busy_times/{staff_id}/{date}")]
        public async Task<Answer<TimeSpan[]>> GetBusyTime(int staff_id, DateTime date)
        {
            return await service.GetStaffBusyTime(staff_id, date);
        }

        [HttpPost("search/{name}")]
        public async Task<Answer<viEvents[]>> SearchAsync(string name)
        {
            return await service.SearchScheduler(name);
        }
    }
}
