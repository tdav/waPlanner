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
    [SwaggerTag("Регистрация")]
    public class RegistrationController
    {
        private readonly IRegistrationService service;

        public RegistrationController(IRegistrationService service)
        {
            this.service = service;
        }

        [HttpPost]
        public Task Registration(viRegistration staff)
        {
            return service.RegisterAsync(staff);
        }
    }
}
