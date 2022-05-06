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
    [SwaggerTag("Организация")]
    [Route("[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService service;
        public OrganizationController(IOrganizationService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<Answer<long>> Insert([FromBody] viOrganization organization, string phoneNum)
        {
            return await service.InsertOrganizationAsync(organization, phoneNum);
        }

        [HttpPost("change")]
        public async Task UpdateOrganizationA(viOrganization organziation)
        {
            await service.UpdateOrganizationAsync(organziation);
        }

        [HttpPost("change/{organization}/{status}")]
        public async Task ChangeStatus(int organization, int status)
        {
            await service.UpdateOrganizationStatus(organization, status);
        }

        [HttpGet("Id")]
        public Task<Answer<spOrganization>> GetOrganizationAsync(int id)
        {
            return service.GetOrgByIdAsync(id);
        }

        [HttpGet]
        public Task<Answer<spOrganization[]>> GetAllAsync()
        {
            return service.GetAllOrgsAsync();
        }
    }
}
