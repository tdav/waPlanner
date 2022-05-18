using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;


namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Анализы")]
    public class AnalysisController: ControllerBase
    {
        private readonly IAnalysisService service;

        public AnalysisController(IAnalysisService service)
        {
            this.service = service;
        }

        [HttpPost("{staff_id}")]
        public ValueTask<Answer<viFullAnalysis[]>> GetAllAsync(int staff_id)
        {
            return service.GetStaffAllAnalysis(staff_id);
        }
    }
}
