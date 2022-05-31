using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Публик")]
    public class PublicController: ControllerBase
    {
        private readonly IPublicService service;

        public PublicController(IPublicService service)
        {
            this.service = service;
        }

        [HttpGet("organizations/s{spec_id}")]
        public ValueTask<Answer<viOrganization[]>> GetOrganizationsAsync(int spec_id)
        {
            return service.GetOrganizationsBySpecId(spec_id);
        }

        [HttpGet("staffs/{organization_id}")]
        public ValueTask<Answer<viStaff[]>> GetStaffsAsync(int organization_id)
        {
            return service.GetStaffsByOrgId(organization_id);
        }

        [HttpGet("categories/{organization_id}")]
        public ValueTask<Answer<viCategory[]>> GetCategoriesAsync(int organization_id)
        {
            return service.GetCategoriesByOrgId(organization_id);
        }
    }
}
