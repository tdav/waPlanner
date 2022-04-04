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
    [SwaggerTag("Персонал")]
    public class StaffController: ControllerBase
    {
        private readonly StaffService service;
        public async Task<tbUser> GetStuffById(int id)
        {
            return await service.GetStaffById(id);
        }
    }
    
}
