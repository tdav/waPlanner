using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.Services;


namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Персонал")]
    [Route("[controller]")]
    public class StaffController: ControllerBase
    {
        private readonly IStaffService service;

        public StaffController(IStaffService service)
        {
            this.service = service;
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public ValueTask<Answer<TokenModel>> TokenAsync(LoginModel value)
        {
            return service.TokenAsync(value);
        }

        [HttpGet("by_organization_id")]
        public Task<Answer<viStaff[]>> GetStuffById()
        {
            return service.GetStaffsByOrganizationId();
        }

        [HttpGet("name_list/{category_id}")]
        public Task<Answer<List<IdValue>>> GetStuffList(int category_id)
        {
            return service.GetStuffList(category_id);
        }

        [HttpPost()]
        public async Task AddStaff(viStaff staff)
        {
            await service.AddStaffAsync(staff);
        }

        [HttpPost("change/{staff_id}/{status}")]
        public async Task ChagneStaffStatus(int staff_id, int status)
        {
            await service.SetStatusAsync(staff_id, status);
        }

        [HttpPost("change")]
        public async Task ChangeStaff(viStaff staff)
        {
            await service.UpdateStaff(staff);
        }
        
        [HttpGet("{staff_id}")]
        public Task<Answer<viStaff>> GetStaffById(int staff_id)
        {
            return service.GetStaffById(staff_id);
        }

        [HttpGet("search/{name}")]
        public Task<Answer<viStaff[]>> SearchStaff(string name)
        {
            return service.SearchStaffAsync(name);
        }

        [HttpPost("set/{staff_id}/{activity}")]
        public async Task SetActivity(int staff_id, bool activity)
        {
            await service.SetActivity(staff_id, activity);
        }

        //[HttpGet("{staff_id}/availability")]
        //public Task<viStaffAvailability> GetAvailability(int staff_id)
        //{
        //    return service.GetStaffAvailabilityAsync(staff_id);
        //}

        //[HttpPost("change_password")]
        //public async ValueTask<AnswerBasic> ChangePassword()
        //{
        //    return await service.ChangePaswwordAsync();
        //}
    }
    
}
