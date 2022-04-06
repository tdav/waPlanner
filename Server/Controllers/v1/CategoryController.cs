using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [SwaggerTag("Категория")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService service;

        public CategoryController(ICategoryService service)
        {
            this.service = service;
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            service.Delete(id);
        }

        [HttpGet("{id}")]
        public async Task<spCategory> Get(int id)
        {
            return await service.GetAsync(id);
        }

        [HttpGet]
        public async Task<spCategory[]> GetAll()
        {
            return await service.GetAllAsync();
        }

        [HttpPost]
        public async Task Insert([FromBody] spCategory value)
        {
            await service.InsertAsync(value);
        }

        [HttpPut]
        public async Task Update([FromBody] spCategory value)
        {
            await service.UpdateAsync(value);
        }
    }
}