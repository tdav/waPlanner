using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.ModelViews;
using waPlanner.Services;


namespace waPlanner.Controllers.v1
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [SwaggerTag("Сетуп")]
    public class SetupController
    {
        private readonly ISetupService service;

        public SetupController(ISetupService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<viSetup> GetSetup()
        {
            return await service.Get();
        }

        [HttpPost]
        public async Task SetSetup(viSetup setup)
        {
            await service.Set(setup);
        }
    }
}
