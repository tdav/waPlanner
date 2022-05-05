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
        private readonly ITelegramGroupCreatorService service;

        public TelegramClientController(ITelegramGroupCreatorService service)
        {
            this.service = service;
        }
                

        [HttpPost("CreateGroup")]
        public async Task<long> CreateGroupAsync(string phoneNum, string orgName)
        {
            return await service.CreateGroup(phoneNum, orgName);
        }

        [HttpPost("GetAuthenticationCode")]
        public Task GetAuthenticationCode()
        {
           return service.GetAuthenticationCode();
        }

        [HttpPost("SetAuthenticationCode")]
        public Task SetAuthenticationCode(string code, string password)
        {
            return service.SetAuthenticationCode(code, password);
        }
    }
}
