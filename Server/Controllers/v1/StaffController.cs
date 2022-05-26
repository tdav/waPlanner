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

        [HttpGet]
        public Task<Answer<viStaff[]>> GetStuffs()
        {
            return service.GetStaffsByOrganizationId();
        }

        [HttpGet("category/{category_id}")]
        public async Task<Answer<viStaff[]>> GetStaffByCategory(int category_id)
        {
            return await service.GetStaffsByCategoryId(category_id);
        }

        [HttpGet("name_list/{category_id}")]
        public Task<Answer<List<IdValue>>> GetStuffList(int category_id)
        {
            return service.GetStuffList(category_id);
        }

        [HttpPost()]
        public async Task<Answer<viStaff>> AddStaff([FromBody] viStaff staff)
        {
            return await service.AddStaffAsync(staff);
        }

        [HttpPost("change_status")]
        public async Task<AnswerBasic> ChagneStaffStatus(viSetStatus status)
        {
            return await service.SetStatusAsync(status);
        }

        [HttpPost("change")]
        public async Task<Answer<viStaff>> ChangeStaff([FromBody] viStaff staff)
        {
            return await service.UpdateStaff(staff);
        }
        
        [HttpGet("{staff_id}")]
        public Task<Answer<viStaff>> GetStaffById(int staff_id)
        {
            return service.GetStaffById(staff_id);
        }

        [HttpGet("search/{name}")]
        public ValueTask<Answer<viStaff[]>> SearchStaff(string name)
        {
            return service.SearchStaffAsync(name);
        }

        [HttpPost("set/{staff_id}/{activity}")]
        public async Task<AnswerBasic> SetActivity(int staff_id, bool activity)
        {
            return await service.SetActivity(staff_id, activity);
        }

        //[HttpGet("{staff_id}/availability")]
        //public Task<viStaffAvailability> GetAvailability(int staff_id)
        //{
        //    return service.GetStaffAvailabilityAsync(staff_id);
        //}

        [HttpPost("change_password")]
        public ValueTask<AnswerBasic> ChangePassword(ChangePasswordModel value)
        {
            return service.ChangePaswwordAsync(value);
        }

        [AllowAnonymous]
        [HttpPost("forgot_password")]
        public ValueTask<Answer<IdValue>> ForgotPassword(string PhoneNum)
        {
            return service.OnForgotPassword(PhoneNum);
        }

        [HttpPost("set_photo")]
        public async ValueTask<Answer<string>> SetPhotoAsync(viSetPhoto photo)
        {
            return await service.SetPhotoAsync(photo);
        }
    }
    
}
