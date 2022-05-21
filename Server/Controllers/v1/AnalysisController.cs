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

        [HttpGet("{user_id}")]
        public ValueTask<Answer<viFullAnalysis[]>> GetAllAsync(int user_id)
        {
            return service.GetStaffAllAnalysis(user_id);
        }

        [HttpGet("delete/{id}")]
        public ValueTask<AnswerBasic> DeleteAsync(int id)
        {
            return service.DeleteAnylysis(id);
        }
    }
}
