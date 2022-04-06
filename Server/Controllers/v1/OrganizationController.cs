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
    [SwaggerTag("Организация")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService service;
        public OrganizationController(IOrganizationService service)
        {
            this.service = service;
        }
        [HttpPost]
        public async Task Insert([FromBody] tbOrganization organization)
        {
            await service.InsertAsync(organization);
        }
        [HttpPut]
        public async Task Update([FromBody] tbOrganization organization)
        {
            await service.UpdateAsync(organization);
        }
        [HttpDelete("id")]
        public void Delete(int id)
        {
            service.Delete(id);
        }
        [HttpGet("Id")]
        public async Task<tbOrganization> GetOrganizationAsync(int id)
        {
            return await service.GetOrgByIdAsync(id);
        }
        [HttpGet]
        public async Task<tbOrganization[]> GetAllAsync()
        {
            return await service.GetAllOrgsAsync();
        }
    }
}
