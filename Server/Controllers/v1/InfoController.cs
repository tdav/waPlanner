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
    [Route("[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Инфо Дашборд")]
    public class InfoController
    {
        private readonly IInfoService service;
        public InfoController(IInfoService service)
        {
            this.service = service;
        }

        [HttpGet("total_today_appointments")]
        public Task<Answer<int>> GetTotalTodayAppointments()
        {
            return service.GetTotalTodayAppointments();
        }

        [HttpGet("week_appointments")]
        public Task<Answer<int>> GetTotalWeekAppointments()
        {
            return service.GetTotalWeekAppointments();
        }

        [HttpGet("today_appointments")]
        public Task<Answer<List<viAppointmentsModel>>> GetTodayAppointments()
        {
            return service.GetTodayAppointments();
        }

        [HttpGet("recent_users")]
        public Task<Answer<List<viRecentSchedulers>>> GetRecentUsers()
        {
            return service.GetRecentUsers();
        }

        [HttpGet("scheduler_diagramma")]
        public Task<Answer<List<viSchedulerDiagramma>>> GetSchedulerDiagramma()
        {
            return service.GetSchedulerDiagramma();
        }
    }
}
