using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using waPlanner.Database.Models;
using waPlanner.ModelViews;
using waPlanner.Services;

namespace waPlanner.Controllers.v1
{
    [Authorize]
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

        [HttpGet("{category_id}")]
        public async Task<viCategory> Get(int category_id)
        {
            return await service.GetCategoryByIdAsync(category_id);
        }

        [HttpGet("get_by_organization/{organization_id}")]
        public async Task<List<viCategory>> GetAll(int organization_id)
        {

            return await service.GetAllCategoryByOrgAsync(organization_id);
        }

        [HttpPost]
        public async Task Insert([FromBody] viCategory value)
        {
            await service.AddCategoryAsync(value);
        }

        [HttpPost("change_category")]
        public async Task Update([FromBody] viCategory value)
        {
            await service.UpdateAsync(value);
        }

        [HttpPost("change_status/{status}")]
        public async Task ChangeCategoryStatus([FromBody] viCategory value, int status)
        {
            await service.ChangeCategoryStatus(value, status);
        }

        [HttpGet]
        public async Task<List<spCategory>> GetAllCats()
        {
            return await service.GetAllCategories();
        }
    }
}