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

        [HttpGet("get_stuff_by_organization_id/{organization_id}")]
        public  Task<List<viStaff>> GetStuffById(int organization_id)
        {
            return service.GetStaffByOrganizationId(organization_id);
        }

        [HttpGet("get_stuff/{organization_id}")]
        public Task<List<IdValue>> GetStuffList(int organization_id)
        {
            return service.GetStuffList(organization_id);
        }

        [HttpPost("add_staff")]
        public async Task AddStaff(viStaff staff)
        {
            await service.AddStaffAsync(staff);
        }

        [HttpPost("change_status/{staff_id}/{status}")]
        public async Task ChagneStaffStatus(int staff_id, byte status)
        {
            await service.UpdateStaffStatus(staff_id, status);
        }

        [HttpPut("change_staff_info/{staff_id}")]
        public async Task ChangeStaff(int staff_id, viStaff staff)
        {
            await service.UpdateStaff(staff_id, staff);
        }
        
        [HttpGet("get_staff_by_id/{staff_id}")]
        public Task<viStaff> GetStaffById(int staff_id)
        {
            return service.GetStaffById(staff_id);
        }
    }
    
}
