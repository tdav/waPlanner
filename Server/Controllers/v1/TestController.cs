using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using waPlanner.ModelViews;

namespace waPlanner.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("[controller]")]
    [SwaggerTag("Тест")]
    public class TestController : ControllerBase
    {

        [HttpGet]
        public LangsModel Get()
        {
            var r = new LangsModel();
            r.Add("N1", new Dictionary<string, string>() { { "UZ", "QI1" } });
            r.Add("N2", new Dictionary<string, string>() { { "UZ", "QI1" } });
            r.Add("N3", new Dictionary<string, string>() { { "UZ", "QI1" } });

            return r;
        }


    }
}