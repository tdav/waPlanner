using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Database.Models;
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

        public FileController(IFileService service)
        {
            this.service = service;
        }

        [HttpPost("save")]
        [Consumes("multipart/form-data")]
        public async ValueTask<Answer<string>> PostFile(IFormFile fileForm)
        {
            return await service.SaveFile(fileForm);
        }


        [HttpPost("save-analize-result")]
        [Consumes("multipart/form-data")]
        public async ValueTask<Answer<tbAnalizeResult>> PostAnalizeResultFile([FromForm] viAnalizeResultFile fileForm)
        {
            return await service.SaveAnalizeResultFile(fileForm);
        }
    }
}
