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
    public class UserController : ControllerBase
    {
        private readonly IUserService service;

        public UserController(IUserService service)
        {
            this.service = service;
        }
        [HttpPost]
        public async Task Insert([FromBody] tbUser user)
        {
            await service.InsertAsync(user);
        }
        [HttpPut]
        public async Task Update([FromBody] tbUser user)
        {
            await service.UpdateAsync(user);
        }
        [HttpDelete("id")]
        public void Delete(int id)
        {
            service.Delete(id);
        }
        [HttpGet("id")]
        public async Task<tbUser> GetUserById(int id)
        {
            return await service.GetUserByIdAsync(id);
        }
        [HttpGet]
        public async Task<tbUser[]> GetAll()
        {
            return await service.GetAllAsync();
        }
    }
}
