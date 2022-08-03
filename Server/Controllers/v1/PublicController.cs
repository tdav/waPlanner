using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Публик")]
    public class PublicController : ControllerBase
    {
        private readonly IPublicService service;

        public PublicController(IPublicService service)
        {
            this.service = service;
        }

        [HttpGet("organizations/{spec_id}")]
        public ValueTask<Answer<List<viOrganization>>> GetOrganizationsBySpecAsync(int spec_id)
        {
            return service.GetOrganizationsBySpecId(spec_id);
        }

        [HttpGet("staffs/{organization_id}")]
        public ValueTask<Answer<List<viStaff>>> GetStaffsAsync(int organization_id)
        {
            return service.GetStaffsByOrgId(organization_id);
        }

        [HttpGet("categories/{organization_id}")]
        public ValueTask<Answer<List<viCategory>>> GetCategoriesAsync(int organization_id)
        {
            return service.GetCategoriesByOrgId(organization_id);
        }

        [HttpGet("all_organizations")]
        public ValueTask<Answer<List<viOrganization>>> GetOrganizations()
        {
            return service.GetOrganizations();
        }

        [HttpGet("organization/{id}")]
        public ValueTask<Answer<viOrganization>> GetOrganization(int id)
        {
            return service.GetOrganizationById(id);
        }

        [HttpGet("search/{param}")]
        public ValueTask<Answer<viPublicSearch>> Search(string param)
        {
            return service.PublicSearch(param);
        }
    }
}
