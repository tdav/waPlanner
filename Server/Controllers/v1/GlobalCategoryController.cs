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
    [SwaggerTag("Глобальная категория")]
    public class GlobalCategoryController: ControllerBase
    {
        private readonly IGlobalCategoryService service;
        public GlobalCategoryController(IGlobalCategoryService service)
        {
            this.service = service;
        }
        [HttpPost]
        public async Task InsertAsync([FromBody] spGlobalCategory globalCategory)
        {
            await service.InsertAsync(globalCategory);
        }
        [HttpPut]
        public async Task UpdateAsync([FromBody] spGlobalCategory globalCategory)
        {
            await service.UpdateAsync(globalCategory);
        }
        [HttpDelete]
        public void Delete([FromBody] spGlobalCategory globalCategory)
        {
            service.Delete(globalCategory);
        }
        [HttpDelete("id")]
        public void Delete(int id)
        {
            service.Delete(id);
        }
        [HttpGet("id")]
        public async Task<spGlobalCategory> GetByIdAsync(int id)
        {
            return await service.GetByIdAsync(id);
        }
        [HttpGet]
        public async Task<spGlobalCategory[]> GetAllAsync()
        {
            return await service.GetAllAsync();
        }
    }
}
