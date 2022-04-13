using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [SwaggerTag("Фойдаланувчилар")]
    public class UserController : ControllerBase
    {
        private readonly IUserService service;
        public UserController(IUserService service)
        {
            this.service = service;
        }

        [HttpGet("get")]
        public ValueTask<Answer<tbUser>> Get(int id)
        {
            return service.Get(id);
        }

        [HttpGet("get_all")]
        public ValueTask<Answer<tbUser[]>> GetAll()
        {
            return service.GetAll();
        }

        [HttpPost("change-password")]
        public ValueTask<AnswerBasic> ChangePassword(ChangePasswordModel value)
        {
            return service.ChangePassword(value);
        }

        [HttpPost("change-status/{user_id}/{status}")]
        public ValueTask<AnswerBasic> ChangeStatus(int user_id, int status)
        {
            return service.ChangeStatus(user_id, status);
        }

        [AllowAnonymous]
        [HttpPost("new")]
        public ValueTask<Answer<int>> CreateUser(tbUser u)
        {
            return service.CreateUser(u);
        }


        [AllowAnonymous]
        [HttpPost("token")]
        public ValueTask<Answer<TokenModel>> TokenAsync(LoginModel value)
        {
            return service.TokenAsync(value);
        }
    }
}
