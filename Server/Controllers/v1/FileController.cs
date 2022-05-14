using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Extensions;
using waPlanner.ModelViews;

namespace waPlanner.Controllers.v1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Файллар билан ишлаш")]
    public class FileController : ControllerBase
    {
        private readonly IFileService service;        
        private readonly IHttpContextAccessorExtensions accessor;

        public FileController(IFileService service, IHttpContextAccessorExtensions accessor)
        {
            this.service = service;            
            this.accessor = accessor;
        }

        [HttpPost("save")]
        [Consumes("multipart/form-data")]
        public async ValueTask<Answer<string>> PostFile(IFormFile fileForm)
        {
            //var roles = accessor.GetRoles();
            //if (!roles.Contains("2")) return new Answer<string>(false, "Role Продавец булиши керак", null);

            var uid = accessor.GetId();

            var info = await service.SaveFile(fileForm, uid);
            
            return info;
        }


        [HttpPost("send-analize-result")]
        [Consumes("multipart/form-data")]
        public async ValueTask<Answer<string>> PostAnalizeResultFile([FromForm]viAnalizeResultFile fileForm)
        {
            //var roles = accessor.GetRoles();
            //if (!roles.Contains("2")) return new Answer<string>(false, "Role Продавец булиши керак", null);

            var uid = accessor.GetId();

            var info = await service.SaveAnalizeResultFile(fileForm, uid);

            return info;
        }
    }
}
