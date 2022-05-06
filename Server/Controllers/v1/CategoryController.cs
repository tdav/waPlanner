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
    [SwaggerTag("Категория")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService service;        

        public CategoryController(ICategoryService service)
        {
            this.service = service;
        }

        [HttpGet("{category_id}")]
        public async Task<Answer<viCategory>> Get(int category_id)
        {
            return await service.GetCategoryByIdAsync(category_id);
        }

        [HttpGet("all_categories")]
        public async Task<Answer<viCategory[]>> GetAll()
        {

            return await service.GetAllCategoriesAsync();
        }

        [HttpPost]
        public async Task<Answer<int>> Insert([FromBody] viCategory value)
        {
            return await service.AddCategoryAsync(value);
        }

        [HttpPost("change")]
        public async Task Update([FromBody] viCategory value)
        {
            await service.UpdateAsync(value);
        }

        [HttpPost("change/{category_id}/{status}")]
        public async Task ChangeCategoryStatus(int category_id, int status)
        {
            await service.ChangeCategoryStatus(category_id, status);
        }

        [HttpGet("search/{name}")]
        public async Task<Answer<viCategory[]>> SearchCategory(string name)
        {
            return await service.SearchCategory(name);
        }
    }
}