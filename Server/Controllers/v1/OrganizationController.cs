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
        public async Task<Answer<long>> Insert([FromBody] viOrganization organization)
        {
            return await service.InsertOrganizationAsync(organization);
        }

        [HttpPost("change")]
        public async Task<AnswerBasic> UpdateOrganizationA([FromBody] spOrganization organziation)
        {
            return await service.UpdateOrganizationAsync(organziation);
        }

        [HttpPost("change_status")]
        public async Task<AnswerBasic> ChangeStatus(viSetStatus status)
        {
            return await service.UpdateOrganizationStatus(status);
        }

        [HttpGet("{id}")]
        public Task<Answer<spOrganization>> GetOrganizationAsync(int id)
        {
            return service.GetOrgByIdAsync(id);
        }

        [HttpGet]
        public Task<Answer<List<spOrganization>>> GetAllAsync()
        {
            return service.GetAllOrgsAsync();
        }
    }
}
