//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Service.Interfaces;
//using Swashbuckle.AspNetCore.Annotations;
//using System.Threading.Tasks;
//using waPlanner.Database.Models;
//using waPlanner.ModelViews;

//namespace waPlanner.Controllers.v1
//{
//    [Authorize]
//    [ApiController]
//    [ApiVersion("1.0")]
//    [Route("api/[controller]")]
//    [SwaggerTag("Фойдаланувчилар тури")]
//    public class UserTypeController : ControllerBase
//    {
//        private readonly IBaseSerive<spUserType> service;

//        public UserTypeController(IBaseSerive<spUserType> service)
//        {
//            this.service = service;
//        }

//        [HttpDelete("{id}")]
//        public ValueTask<AnswerBasic> Delete(int id)
//        {
//            return service.Delete(id);
//        }

//        [AllowAnonymous]
//        [HttpGet]
//        public ValueTask<Answer<spUserType[]>> Get()
//        {
//            return service.Get();
//        }

//        [AllowAnonymous]
//        [HttpGet("{id}")]
//        public ValueTask<Answer<spUserType>> Get(int id)
//        {
//            return service.Get(id);
//        }

//        [HttpPost]
//        public ValueTask<Answer<spUserType>> Post([FromBody] spUserType value)
//        {
//            return service.Post(value);
//        }

//        [HttpPut]
//        public ValueTask<AnswerBasic> Put([FromBody] spUserType value)
//        {
//            return service.Put(value);
//        }
//    }
//}