﻿using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Route("api/[controller]")]
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
        public Task<int> GetTotalTodayAppointments(int organization_id)
        {
            return service.GetTotalTodayAppointments(organization_id);
        }
        [HttpGet("week_appointments")]
        public Task<int> GetTotalWeekAppointments(int organization_id)
        {
            return service.GetTotalWeekAppointments(organization_id);
        }
        [HttpGet("today_appointments")]
        public Task<List<viAppointmentsModel>> GetTodayAppointments(int organization_id)
        {
            return service.GetTodayAppointments(organization_id);
        }
        [HttpGet("recent_users")]
        public Task<List<viRecentSchedulers>> GetRecentUsers(int organization_id)
        {
            return service.GetRecentUsers(organization_id);
        }
        [HttpGet("scheduler_diagramma")]
        public Task<List<viSchedulerDiagramma>> GetSchedulerDiagramma(int organization_id)
        {
            return service.GetSchedulerDiagramma(organization_id);
        }
    }
}
