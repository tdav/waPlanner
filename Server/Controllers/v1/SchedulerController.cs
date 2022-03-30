using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Пользователи")]
    public class SchedulerController: ControllerBase
    {
        private readonly ISchedulerService service;
        public SchedulerController(ISchedulerService service)
        {
            this.service = service;
        }
        [HttpPost]
        public async Task Insert([FromBody] tbScheduler scheduler)
        {
            await service.InsertAsync(scheduler);
        }
        [HttpPut]
        public async Task Update([FromBody] tbScheduler scheduler)
        {
            await service.UpdateAsync(scheduler);
        }
        [HttpDelete]
        public void Delete([FromBody] tbScheduler scheduler)
        {
            service.Delete(scheduler);
        }
        [HttpDelete("id")]
        public void Delete(int id)
        {
            service.Delete(id);
        }
        [HttpGet]
        public async Task<tbScheduler> GetSchedulerById(int id)
        {
            return await service.GetSchedulerByIdAsync(id);
        }
        [HttpGet("id")]
        public async Task<tbScheduler[]> GetSchedulers()
        {
            return await service.GetAllSchedulersAsync();
        }
    }
}
