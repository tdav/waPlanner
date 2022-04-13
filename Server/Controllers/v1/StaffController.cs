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

        [HttpGet("get_stuffs_by_organization_id/{organization_id}")]
        public  Task<List<viStaff>> GetStuffById(int organization_id)
        {
            return service.GetStaffByOrganizationId(organization_id);
        }

        [HttpGet("get_stuff_name_list/{organization_id}")]
        public Task<List<IdValue>> GetStuffList(int organization_id)
        {
            return service.GetStuffList(organization_id);
        }

        [HttpPost("add_staff")]
        public async Task AddStaff(viStaff staff, int organization_id)
        {
            await service.AddStaffAsync(staff, organization_id);
        }

        [HttpPost("change_status/{staff_id}/{status}")]
        public async Task ChagneStaffStatus(viStaff staff, byte status)
        {
            await service.UpdateStaffStatus(staff, status);
        }

        [HttpPost("change_staff_info")]
        public async Task ChangeStaff(viStaff staff)
        {
            await service.UpdateStaff(staff);
        }
        
        [HttpGet("get_staff_by_id/{staff_id}")]
        public Task<viStaff> GetStaffById(int staff_id)
        {
            return service.GetStaffById(staff_id);
        }
    }
    
}
