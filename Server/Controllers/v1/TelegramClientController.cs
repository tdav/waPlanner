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
    [SwaggerTag("Тг Клиент")]
    public class TelegramClientController: ControllerBase
    {
        private readonly ITelegramClientActivate service;

        public TelegramClientController(ITelegramClientActivate service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task Activate
    }
}
