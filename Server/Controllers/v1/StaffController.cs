using Microsoft.AspNetCore.Mvc;
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
    [SwaggerTag("Персонал")]
    public class StaffController: ControllerBase
    {
        private readonly IStaffService service;
        public StaffController(IStaffService service)
        {
            this.service = service;
        }
        [HttpGet("GetStuffById/{organization_id}")]
        public  Task<List<viUser>> GetStuffById(int organization_id)
        {
            return service.GetStaffById(organization_id);
        }
    }
    
}
