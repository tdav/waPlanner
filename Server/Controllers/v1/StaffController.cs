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
        public  Task<viStaff[]> GetStuffById()
        {
            return service.GetStaffByOrganizationId();
        }

        [HttpGet("name_list/{category_id}")]
        public Task<List<IdValue>> GetStuffList(int category_id)
        {
            return service.GetStuffList(category_id);
        }

        [HttpPost()]
        public async Task AddStaff(viStaff staff)
        {
            await service.AddStaffAsync(staff);
        }

        [HttpPost("change/{status}")]
        public async Task ChagneStaffStatus(viStaff staff, int status)
        {
            await service.SetStatusAsync(staff, status);
        }

        [HttpPost("change")]
        public async Task ChangeStaff(viStaff staff)
        {
            await service.UpdateStaff(staff);
        }
        
        [HttpGet("{staff_id}")]
        public Task<viStaff> GetStaffById(int staff_id)
        {
            return service.GetStaffById(staff_id);
        }

        //[HttpPost("change_password")]
        //public async ValueTask<AnswerBasic> ChangePassword()
        //{
        //    return await service.ChangePaswwordAsync();
        //}
    }
    
}
