using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Организация")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService service;
        public OrganizationController(IOrganizationService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task Insert([FromBody] viOrganization organization)
        {
            await service.InsertOrganizationAsync(organization);
        }

        [HttpPut("change_organization/{organization_id}")]
        public async Task UpdateOrganizationA(int organization_id, viOrganization organziation)
        {
            await service.UpdateOrganizationAsync(organization_id, organziation);
        }

        [HttpPut("change_organization_status/{organization_id}/{status}")]
        public async Task ChangeStatus(int organization_id, int status)
        {
            await service.UpdateOrganizationStatus(organization_id, status);
        }

        [HttpGet("Id")]
        public Task<spOrganization> GetOrganizationAsync(int id)
        {
            return service.GetOrgByIdAsync(id);
        }

        [HttpGet]
        public Task<spOrganization[]> GetAllAsync()
        {
            return service.GetAllOrgsAsync();
        }
    }
}
